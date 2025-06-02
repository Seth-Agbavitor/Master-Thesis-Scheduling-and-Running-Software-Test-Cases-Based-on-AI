using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace SimPaintRobot
{
    [TestClass]
    public class PaintRobotTests
    {
        private PaintRobot robot;
        private static readonly Random delayRandom = new Random();

        private void RandomDelay() => Thread.Sleep(delayRandom.Next(100, 2000));
        private void Delay30Seconds() => Thread.Sleep(30000);
        private void Delay60Seconds() => Thread.Sleep(60000);
        private void Delay120Seconds() => Thread.Sleep(120000);

        [TestMethod]
        public void PaintABB5400_MotionRangeCheck()
        {
            robot = new PaintRobot(0.2); // 20% failure probability
            Delay60Seconds();
            Assert.IsTrue(robot.MoveTo(600, 300, 200));
        }

        [TestMethod]
        public void PaintIRB5500_ResetAfterFailure()
        {
            robot = new PaintRobot(0.5); // 50% failure probability
            RandomDelay();
            robot.InjectRandomFailure();
            robot.Reset();
            Assert.IsTrue(robot.MoveTo(200, 200, 200));
        }

        [TestMethod]
        public void PaintIRB5400_SequenceError()
        {
            robot = new PaintRobot(0.4);
            Delay30Seconds();
            robot.MoveTo(300, 300, 300);
            robot.InjectRandomFailure();
            Assert.IsFalse(robot.StartPainting());
        }

        [TestMethod]
        public void PaintIRB580_TriggerFailsafe()
        {
            robot = new PaintRobot(0.6);
            RandomDelay();
            robot.MoveTo(400, 200, 100);
            robot.InjectRandomFailure();
            Assert.IsFalse(robot.MoveTo(100, 100, 100));
        }

        [TestMethod]
        public void ABBPaintingBot_RecoveryFromCrash()
        {
            robot = new PaintRobot(0.3);
            Delay30Seconds();
            robot.InjectRandomFailure();
            robot.Reset();
            Assert.IsTrue(robot.StartPainting());
        }

        [TestMethod]
        public void HighPrecisionArm_BoundaryFailure()
        {
            robot = new PaintRobot(0.8); // 80% failure probability for precision failure
            RandomDelay();
            Assert.IsFalse(robot.MoveTo(-1, 0, 0));
        }

        [TestMethod]
        public void CompactPaintBot_QuickTest()
        {
            robot = new PaintRobot(0.1); // Low failure probability
            RandomDelay();
            Assert.IsTrue(robot.MoveTo(100, 100, 100));
        }

        [TestMethod]
        public void SmartPainter_PositionPrecision()
        {
            robot = new PaintRobot(0.5);
            RandomDelay();
            Assert.IsTrue(robot.MoveTo(999, 999, 999));
        }

        [TestMethod]
        public void ABBPaintBot_TriggerFailsafeFlow()
        {
            robot = new PaintRobot(0.7);
            RandomDelay();
            robot.MoveTo(100, 100, 100);
            robot.InjectRandomFailure();
            Assert.IsTrue(robot.IsInFailsafe);
        }
    }

    public class PaintRobot
    {
        public (int X, int Y, int Z) CurrentPosition { get; private set; } = (0, 0, 0);
        public bool IsPainting { get; private set; } = false;
        public bool IsInFailsafe { get; private set; } = false;

        private static Random rnd = new Random();
        private readonly double failureChance;

        public PaintRobot(double failureChance)
        {
            this.failureChance = failureChance;
        }

        public bool MoveTo(int x, int y, int z)
        {
            if (IsInFailsafe || x < 0 || x > 1000 || y < 0 || y > 1000 || z < 0 || z > 1000)
            {
                IsInFailsafe = true;
                return false;
            }

            if (rnd.NextDouble() < failureChance)
            {
                IsInFailsafe = true;
                return false;
            }

            CurrentPosition = (x, y, z);
            return true;
        }

        public bool StartPainting()
        {
            if (IsInFailsafe || IsPainting)
                return false;

            if (rnd.NextDouble() < failureChance)
            {
                IsInFailsafe = true;
                return false;
            }

            IsPainting = true;
            return true;
        }

        public bool StopPainting()
        {
            if (IsInFailsafe || !IsPainting)
                return false;

            IsPainting = false;
            return true;
        }

        public void InjectRandomFailure()
        {
            IsInFailsafe = true;
        }

        public void Reset()
        {
            IsInFailsafe = false;
            IsPainting = false;
            CurrentPosition = (0, 0, 0);
        }
    }
}
