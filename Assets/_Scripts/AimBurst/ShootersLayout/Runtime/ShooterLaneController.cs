using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AimBurst.Core.Contracts;
using AimBurst.ShootersLayout.Contracts;
using AimBurst.ShootersLayout.Runtime.Abstractions;

namespace AimBurst.ShootersLayout.Runtime {
    internal sealed class ShooterLaneController : ILaneController, ILaneState {
        private readonly List<IShooterLane> shooters;
        private readonly float shooterSpacing;
        private IShooterLane frontShooter;
        private bool isAdvancing;

        public int CurrentShootersCount => shooters.Count;
        public CubeColor? NextShooterColor => shooters.Count > 1 ? shooters[1].Color : null;

        public static void ClearInstances() => Instances.Clear();
        private static readonly Dictionary<int, ShooterLaneController> Instances = new Dictionary<int, ShooterLaneController>();

        public static async Task AdvanceLane(int laneIndex) {
            var controller = Instances[laneIndex];
            await controller.AdvanceLaneAsync();
        }

        public static IShooterLane Dequeue(int laneIndex) {
            var controller = Instances[laneIndex];
            return controller.Dequeue();
        }

        public ShooterLaneController(int index, List<IShooterLane> shooters, float shooterSpacing) {
            this.shooters = shooters;
            this.shooterSpacing = shooterSpacing;
            if (Instances.ContainsKey(index)) { throw new InvalidOperationException($"<color=white>[{nameof(ShooterLaneController)}] Cannot add same index twice"); }
            Instances.Add(index, this);
        }

        internal void Init() {
            for (int i = 0; i < shooters.Count; i++) {
                IShooterLane shooter = shooters[i];
                if (i == 0) { frontShooter = shooter; }
                var positioning = i == 0 ? ShooterPositioning.Front : ShooterPositioning.Lane;
                shooter.SetPositioning(positioning);
            }
            isAdvancing = false;
        }

        public IShooterLane Peek() {
            if (shooters.Count == 0) { return null; }
            return frontShooter;
        }

        public IShooterLane Dequeue() {
            shooters.Remove(frontShooter);
            frontShooter.SetPositioning(ShooterPositioning.Slot);
            if (shooters.Count == 0) { return null; }
            return frontShooter;
        }

        public async Task AdvanceLaneAsync() {
            if (isAdvancing) { return; }
            if (shooters.Count == 0) { return; }

            isAdvancing = true;
            await MoveLaneForward();
            isAdvancing = false;
            return;
        }

        private async Task MoveLaneForward() {
            //Dequeue();
            if (shooters.Count > 0) {
                frontShooter = shooters[0];

                Task[] moveTasks = new Task[shooters.Count];
                for (int i = 0; i < shooters.Count; i++) {
                    var positioning = i == 0 ? ShooterPositioning.Front : ShooterPositioning.Lane;
                    shooters[i].SetPositioning(positioning);
                    moveTasks[i] = shooters[i].Advance(shooterSpacing);
                }

                await Task.WhenAll(moveTasks);
            }
        }
    }
}
