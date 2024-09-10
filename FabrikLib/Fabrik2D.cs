using Godot;
using System;
using System.Linq;

namespace FabrikLib {
    public class Fabrik2D {
        public float tolerance = 0.1f;
        public int maxIterations = 10;

        public Vector2[] chain;

        public Vector2[] chainDirections { 
            get {
                if (chain == null) {
                    throw new Exception("Fabrik. input chain is null. Cannot get chainDirections!");
                }
                var directions = new Vector2[chain.Length];
                for (int i = 0; i < chain.Length - 1; i++) {
                    directions[i] = chain[i].DirectionTo(chain[i + 1]);
                }
                return directions;
            } 
        }
        
        public Vector2[] chainJointAngleMinMax;

        private float[] chainMagnitudes;
        private float maxChainMagnitudeSquared;
        private Vector2 origin;

        public Fabrik2D() {

        }

        public Fabrik2D(Vector2[] chain) {
            this.chain = chain;
            this.chainMagnitudes = new float[chain.Length];
            this.chainMagnitudes[0] = 0f;
            for (int i = 1; i < chain.Length; i++) {
                this.chainMagnitudes[i] = chain[i - 1].DistanceTo(chain[i]);
            }

            this.maxChainMagnitudeSquared = chainMagnitudes.Sum() * chainMagnitudes.Sum();
            this.origin = chain[0];
        }

        public Vector2[] solve(Vector2 target) {
            if (maxChainMagnitudeSquared < target.DistanceSquaredTo(this.origin)) {
                beelineSolve(target);
                return chain;
            }

            for (int i = 0; i < maxIterations; i++) {
                if (chain[chain.Length - 1].DistanceSquaredTo(target) < tolerance)
                    break;

                backwards(target);
                forward();
            }
            return chain;
        }

        private void backwards(Vector2 target) {
            chain[chain.Length - 1] = target;
            Vector2 previousSegment;
            for (int i = chain.Length - 2; i >= 0; i--) {
                previousSegment = chain[i + 1];
                var unclampedDirection = previousSegment.DirectionTo(chain[i]);
                chain[i] = previousSegment + (unclampedDirection * chainMagnitudes[i]);

                // calculate angles. This task only starts when calculating for the 3rd chain's position.
                if (i < chain.Length - 2) {
                    var baselineDirection = -previousSegment.DirectionTo(chain[i + 2]);
                    var unclammpedAngle = baselineDirection.AngleTo(unclampedDirection);
                    var clampped = Mathf.Clamp(-unclammpedAngle, chainJointAngleMinMax[i].X, chainJointAngleMinMax[i].Y);
                    chain[i] = previousSegment + (baselineDirection.Rotated(-clampped) * chainMagnitudes[i]);
                }
            }
        }

        private void forward() {
            chain[0] = origin;
            Vector2 previousSegment;
            for (int i = 1; i < chain.Length; i++) {
                previousSegment = chain[i-1];
                var unclampedDirection = previousSegment.DirectionTo(chain[i]);
                chain[i] = previousSegment + (unclampedDirection * chainMagnitudes[i]);

                if (i > 1) {
                    var baselineDirection = -previousSegment.DirectionTo(chain[i - 2]);
                    var unclammpedAngle = baselineDirection.AngleTo(unclampedDirection);
                    var clampped = Mathf.Clamp(-unclammpedAngle, chainJointAngleMinMax[i].X, chainJointAngleMinMax[i].Y);
                    chain[i] = previousSegment + (baselineDirection.Rotated(-clampped) * chainMagnitudes[i]);
                }
            }
        }

        private void beelineSolve(Vector2 target) {
            var direction = origin.DirectionTo(target);
            for (int i = 1; i < chain.Length; i++) {
                chain[i] = chain[i - 1] + direction * chainMagnitudes[i];
            }
        }

        private void calculateJointConstraints() {

        }
    }
}
