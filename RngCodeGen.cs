using Iced.Intel;
using System.IO;
using static Iced.Intel.AssemblerRegisters;

namespace H2Randomizer
{
    public static class RngCodeGen
    {
        public static byte[] GenerateCharRng(RandomizerAllocation alloc, LevelDataAllocation level)
        {
            // original character is in dx
            // squad is in r8 as a multiple of record size (116)
            // squad starting location index is in rdi

            var a = new Assembler(64);
            var allowCheck = a.CreateLabel("allowCheck");
            var randomize = a.CreateLabel("randomize");
            var end = a.CreateLabel("end");

            a.push(rsi);
            a.push(rdi);
            a.push(r12);
            a.push(r13);
            a.push(r14);
            a.push(r9);

            a.mov(r9, alloc.SquadSpawnIndex);
            a.mov(__[r9], rdi);

            // store dx in r9 while we're setting up squad check
            a.xor(r9, r9);
            a.mov(r9w, dx);

            a.mov(esi, 116);
            a.mov(eax, r8d);
            a.xor(rdx, rdx);
            a.div(esi);
            a.mov(rdx, r9); // restore dx back
            a.mov(r9d, eax); // move squad index (result if div) into r9
            a.mov(rsi, alloc.SquadIndex);
            a.mov(__[rsi], eax); // store squad index for later (weapon roll)

            if (level.BannedSquadCount > 0)
            {
                var loop = a.CreateLabel("loop");

                a.mov(ax, dx); // pre-emptively put squad into ax for bail branch

                a.mov(rsi, level.BannedSquads);
                a.mov(rdi, level.BannedSquadCount);
                a.lea(rdi, __[rsi + rdi * 4]);

                a.Label(ref loop);
                a.cmp(r9d, __dword_ptr[rsi]);
                a.je(end); // bail if the current squad is in the ban list

                a.add(rsi, 4);
                a.cmp(rsi, rdi);
                a.jl(loop);
            }

            a.mov(r12, level.CharCount);
            a.mov(r13, alloc.SeedAddress);
            a.mov(r14, level.AllowedChars);
            a.mov(rsi, level.CharIndexes);

            // check if we've been given -1. If so, put into ax and return
            a.cmp(dx, -1);
            a.jne(allowCheck);
            a.mov(ax, dx);
            a.jmp(end);

            a.Label(ref allowCheck);
            a.lea(r14, __dword_ptr[r14 + rdx * 4]);
            a.mov(r14, __dword_ptr[r14]);
            a.cmp(r14d, 1);
            a.je(randomize);
            a.mov(ax, dx);
            a.jmp(end);

            // randomize
            a.Label(ref randomize);
            a.GenerateCoreRng(alloc);

            // do lookup for allowed character
            a.lea(rax, __dword_ptr[rsi + rax * 4]);
            a.mov(eax, __dword_ptr[rax]);

            a.Record(LogType.CharacterChoice, alloc, eax);

            // end
            a.Label(ref end);
            a.pop(r9);
            a.pop(r14);
            a.pop(r13);
            a.pop(r12);
            a.pop(rdi);
            a.pop(rsi);
            a.ret();

            using var ms = new MemoryStream();
            a.Assemble(new StreamCodeWriter(ms), 0);

            return ms.ToArray();
        }

        public static byte[] GenerateWeapRng(RandomizerAllocation alloc, LevelDataAllocation level)
        {
            // original weapon is in cx,
            // result needs to be in r9d and end with a test r9d, r9d for the jump

            var a = new Assembler(64);
            var allowCheck = a.CreateLabel("allowCheck");
            var randomize = a.CreateLabel("randomize");
            var end = a.CreateLabel("end");

            a.push(rax);
            a.push(rdx);
            a.push(rsi);
            a.push(rdi);
            a.push(r12);
            a.push(r13);
            a.push(r14);

            a.xor(r9, r9);

            a.mov(r12, level.WeapCount);
            a.mov(r13, alloc.SeedAddress);
            a.mov(r14, level.AllowedWeaps);
            a.mov(rsi, level.WeapIndexes);

            // check if we've been given -1. If so, put into r9 and return
            a.cmp(cx, -1);
            a.jne(allowCheck);
            a.mov(r9w, cx);
            a.jmp(end);

            a.Label(ref allowCheck);
            a.lea(r14, __dword_ptr[r14 + rcx * 4]);
            a.mov(r14, __dword_ptr[r14]);
            a.cmp(r14d, 1);
            a.je(randomize);
            a.mov(r9w, cx);
            a.jmp(end);

            // randomize
            a.Label(ref randomize);
            a.GenerateCoreRng(alloc);
            

            // do lookup for allowed weapon
            a.lea(rax, __dword_ptr[rsi + rax * 4]);
            a.mov(r9d, __dword_ptr[rax]);

            a.Record(LogType.WeaponChoice, alloc, r9d);

            // end
            a.Label(ref end);
            a.mov(cx, r9w); // put the value back in cx to ensure calling code doesn't break
            a.test(r9d, r9d);
            a.pop(r14);
            a.pop(r13);
            a.pop(r12);
            a.pop(rdi);
            a.pop(rsi);
            a.pop(rdx);
            a.pop(rax);
            a.ret();

            using var ms = new MemoryStream();
            a.Assemble(new StreamCodeWriter(ms), 0);

            return ms.ToArray();
        }

        public static byte[] GenerateWeapHook(nint weapRng, nint rip)
        {
            var ass = new Assembler(64);
            ass.mov(r9, weapRng);
            ass.call(r9);
            ass.js((ulong)rip + 0x3E);
            ass.nop();
            ass.nop();
            ass.nop();

            using var memoryStream = new MemoryStream();
            var result = ass.Assemble(new StreamCodeWriter(memoryStream), (ulong)rip);

            return memoryStream.ToArray();
        }

        public static byte[] GenerateCharHook(nint charRng, nint rip)
        {
            var ass = new Assembler(64);
            ass.mov(rcx, charRng);
            ass.call(rcx);
            ass.mov(dx, ax);
            
            ass.test(dx, dx);
            ass.js((ulong)rip + 9 + 0x183);
            ass.nop();

            using var memoryStream = new MemoryStream();
            var result = ass.Assemble(new StreamCodeWriter(memoryStream), (ulong)rip);

            return memoryStream.ToArray();
        }

        private static void GenerateCoreRng(this Assembler a, RandomizerAllocation alloc)
        {
            a.push(r15);
            a.xor(r15, r15);
            a.mov(rax, alloc.SeedAddress);
            a.imul(eax, __[rax], 0x343fd);
            a.mov(r15, alloc.SquadIndex);
            a.mov(r15, __[r15]);
            a.lea(r15, __[r15 + 1]);
            a.imul(r15d, eax);
            a.imul(eax, r15d, 0x41C64E6D);
            a.mov(r15, alloc.SquadSpawnIndex);
            a.mov(r15, __[r15]);
            a.lea(r15, __[r15 + 1]);
            a.imul(r15d, eax);
            a.imul(eax, r15d, 0x343fd);
            a.add(eax, 0x269ec3);
            a.sar(eax, 0x10);
            a.and(eax, 0x7fff);
            a.cdq();
            a.idiv(r12); // divide by char count
            a.mov(eax, edx); // put remainder in eax
            a.pop(r15);
        }

        public struct LogRecord
        {
            public LogType LogType;
            public int SquadIndex;
            public int SpawnIndex;
            public int ChosenValue;
        }

        private static void Record(this Assembler a, LogType type, RandomizerAllocation alloc, AssemblerRegister32 chosenValue)
        {
            const int recordSize = 16;

            a.push(r12);
            a.push(r13);

            a.xor(r12, r12);
            a.xor(r13, r13);

            a.mov(r12, alloc.LogIndex);
            a.imul(r12d, __dword_ptr[r12], recordSize);
            a.mov(r13, alloc.Logs);
            a.add(r13, r12);

            // 0, LogType
            a.mov(__dword_ptr[r13], (int)type);
            a.add(r13, 4);

            // 4, Squad index
            a.mov(r12, alloc.SquadIndex);
            a.mov(r12d, __dword_ptr[r12]);
            a.mov(__[r13], r12d);
            a.add(r13, 4);

            // 8, Spawn index
            a.mov(r12, alloc.SquadSpawnIndex);
            a.mov(r12d, __dword_ptr[r12]);
            a.mov(__[r13], r12d);
            a.add(r13, 4);

            // 12, Chosen value
            a.mov(__[r13], chosenValue);

            a.mov(r12, alloc.LogIndex);
            a.add(__dword_ptr[r12], 1);

            a.pop(r13);
            a.pop(r12);
        }

        public enum LogType
        {
            CharacterChoice = 1,
            WeaponChoice = 2,
        }
    }
}
