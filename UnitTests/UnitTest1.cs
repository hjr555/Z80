using System;
using System.Threading;
using System.Threading.Tasks;
using ClockAdapter;
using MemoryAdapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z80;
using Z80.Z80.Ports;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private CPU Sut { get; set; }
        private IClock Clock { get; set; }
        private IMemory Memory { get; set; }

        [TestInitialize()]
        public void Initialise()
        {
            Clock = new Clock();
            Memory = new Memory(1024);

            Registers.Initialize();
            Sut = new CPU(Clock, Memory);
        }

        [TestCleanup()]
        public void Cleanup()
        {
            Sut = null;
        }

        [TestMethod]
        public void Noop()
        {
            byte instruction = 0x00;
            ushort address = 0;

            Sut.Poke(instruction, address);

            Sut.ExecuteNextInstruction();

            Assert.AreEqual(4, (int)Clock.GetElapsedCycles());
            Assert.AreEqual(1, Registers.PC);
        }

        private void SetupMemory(byte instruction, byte firstByte = 0, byte secondByte = 0)
        {
            Memory.SetByte(0, instruction);

            if(firstByte != 0)
            {
                Memory.SetByte(1, firstByte);

                if(secondByte != 0) Memory.SetByte(2, secondByte);
            }
        }

        [TestMethod]
        public void LD_bc_A()
        {
            byte instruction = 0x02;
            Registers.A.Value = 99;
            Registers.BC.Value = 500;

            SetupMemory(instruction);

            Sut.ExecuteNextInstruction();

            Assert.AreEqual(99, Memory.GetByte(500));
            Assert.AreEqual(7, (int)Clock.GetElapsedCycles());
            Assert.AreEqual(1, Registers.PC);
        }

        [TestMethod]
        public void INC_BC()
        {
            byte instruction = 0x03;
            Registers.BC.Value = 100;

            SetupMemory(instruction);

            Sut.ExecuteNextInstruction();

            Assert.AreEqual(101, Registers.BC);
            Assert.AreEqual(6, (int)Clock.GetElapsedCycles());
            Assert.AreEqual(1, Registers.PC);
        }

        [TestMethod]
        public void INC_B()
        {
            byte instruction = 0x04;
            Registers.B.Value = 100;

            SetupMemory(instruction);

            Sut.ExecuteNextInstruction();

            Assert.AreEqual(101, Registers.B);
            Assert.AreEqual(4, (int)Clock.GetElapsedCycles());
            Assert.AreEqual(1, Registers.PC);
        }

        [TestMethod]
        public void INC_B_WithParity()
        {
            byte instruction = 0x04;
            Registers.B.Value = byte.MaxValue;

            SetupMemory(instruction);

            Sut.ExecuteNextInstruction();

            Assert.AreEqual(0, Registers.B);
            Assert.AreEqual(4, (int)Clock.GetElapsedCycles());
            Assert.AreEqual(true, Registers.Parity);
            Assert.AreEqual(1, Registers.PC);
        }


        [TestMethod]
        public void DEC_B()
        {
            byte instruction = 0x05;
            Registers.B.Value = 100;

            SetupMemory(instruction);

            Sut.ExecuteNextInstruction();

            Assert.AreEqual(99, Registers.B);
            Assert.AreEqual(4, (int)Clock.GetElapsedCycles());
            Assert.AreEqual(1, Registers.PC);
        }

        [TestMethod]
        public void LD_RR_nn()
        {
            byte instruction = 0x01;

            SetupMemory(instruction, 99, 88);

            Sut.ExecuteNextInstruction();

            Assert.AreEqual(22627, Registers.BC);
            Assert.AreEqual(10, (int)Clock.GetElapsedCycles());
            Assert.AreEqual(3, Registers.PC);
        }
    }
}
