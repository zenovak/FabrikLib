using Godot;
using System;
using System.Linq;

namespace FabrikLib {
    public class Fabrik2D {
        public float tolerance = 0.1f;
        public int maxIterations = 10;

        public Vector2[] chain { 
            get 
                { return chain; } 
            set {
                chain = value;

                this.chainMagnitudes = new float[chain.Length];
                this.chainMagnitudes[0] = 0f;
                for (int i = 1; i < chain.Length; i++) {
                    this.chainMagnitudes[i] = chain[i - 1].DistanceTo(chain[i]);
                }

                this.maxChainMagnitudeSquared = chainMagnitudes.Sum() * chainMagnitudes.Sum();
                this.origin = chain[0];
            }
        }

        private float[] chainMagnitudes;
        private float maxChainMagnitudeSquared;
        private Vector2 origin;

        public Fabrik2D() {

        }

        public Fabrik2D(Vector2[] chain) {
            this.chain = chain;
        }

        public Vector2[] solve(Vector2 target) {
            if (target.DistanceSquaredTo(this.origin) < maxChainMagnitudeSquared)
                return beelineSolve(target);

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

            for (int i = chain.Length - 2; i >= 0; i--) {
                var previousSegment = chain[i+1];
                chain[i] = previousSegment + (previousSegment.DirectionTo(chain[i]) * chainMagnitudes[i]);
            }
        }

        private void forward() {
            chain[0] = origin;

            for (int i = 1; i < chain.Length; i++) {
                var previousSegment = chain[i - 1];
                chain[i] = previousSegment + (previousSegment.DirectionTo(chain[i]) * chainMagnitudes[i]);
            }
        }

        private Vector2[] beelineSolve(Vector2 target) {
            var direction = origin.DirectionTo(target);
            for (int i = 1; i < chain.Length; i++) {
                chain[i] = chain[i] + direction * chainMagnitudes[i];
            }
            return chain;
        }
    }
}
