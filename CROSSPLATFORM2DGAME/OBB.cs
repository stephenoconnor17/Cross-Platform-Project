using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Numerics; // For Vector2


    namespace CROSSPLATFORM2DGAME {
        public class OBB {
            public Vector2 Center { get; private set; }  // Object center
            public double Width { get; private set; }    // Rectangle width
            public double Height { get; private set; }   // Rectangle height
            public double Rotation { get; private set; } // Rotation in radians
            public Vector2[] Corners { get; private set; } = new Vector2[4];

            public OBB(Vector2 center, double width, double height, double rotationRadians = 0) {
                Center = center;
                Width = width;
                Height = height;
                Rotation = rotationRadians;
                UpdateCorners();
            }

            // Call this whenever the object moves or rotates
            public void Update(Vector2 center, double rotationRadians) {
                Center = center;
                Rotation = rotationRadians;
                UpdateCorners();
            }

            // Calculate corner positions based on center, size, and rotation
            private void UpdateCorners() {
                double hw = Width / 2.0;
                double hh = Height / 2.0;

                Corners[0] = RotatePoint(-hw, -hh, Rotation) + Center; // Top-left
                Corners[1] = RotatePoint(hw, -hh, Rotation) + Center;  // Top-right
                Corners[2] = RotatePoint(hw, hh, Rotation) + Center;   // Bottom-right
                Corners[3] = RotatePoint(-hw, hh, Rotation) + Center;  // Bottom-left
            }

            private Vector2 RotatePoint(double x, double y, double theta) {
                float rx = (float)(x * Math.Cos(theta) - y * Math.Sin(theta));
                float ry = (float)(x * Math.Sin(theta) + y * Math.Cos(theta));
                return new Vector2(rx, ry);
            }

            // Check collision with another OBB using SAT
            public bool Intersects(OBB other) {
                Vector2[] axes = GetAxes();
                Vector2[] otherAxes = other.GetAxes();

                foreach (var axis in axes) {
                    if (!OverlapOnAxis(this, other, axis))
                        return false; // No collision
                }

                foreach (var axis in otherAxes) {
                    if (!OverlapOnAxis(this, other, axis))
                        return false;
                }

                return true; // Collision detected
            }

        // Get the 2 normalized axes of this OBB (edge normals)
        /*
        private Vector2[] GetAxes() {
            Vector2[] axes = new Vector2[2];
            axes[0] = Vector2.Normalize(Corners[1] - Corners[0]); // Top edge
            axes[0] = new Vector2(-axes[0].Y, axes[0].X);         // perpendicular
            axes[1] = Vector2.Normalize(Corners[1] - Corners[2]); // Right edge
            axes[1] = new Vector2(-axes[1].Y, axes[1].X);         // perpendicular
            return axes;
        }*/
        public Vector2[] GetAxes() {
            Vector2[] axes = new Vector2[2];

            // Edge 0 -> 1 (top edge)
            Vector2 edge1 = Corners[1] - Corners[0];
            axes[0] = Vector2.Normalize(new Vector2(-edge1.Y, edge1.X)); // perpendicular (normal)

            // Edge 0 -> 3 (left edge)
            Vector2 edge2 = Corners[3] - Corners[0];
            axes[1] = Vector2.Normalize(new Vector2(-edge2.Y, edge2.X)); // perpendicular (normal)

            return axes;
        }

        public Vector2 GetMTV(OBB obstacle) {
            Vector2 mtvAxis = new Vector2();
            float minOverlap = float.MaxValue;

            Vector2[] axes = this.GetAxes().Concat(obstacle.GetAxes()).ToArray();

            foreach (var axis in axes) {
                void Project(OBB obb, Vector2 ax, out float min, out float max) {
                    min = max = Vector2.Dot(obb.Corners[0], ax);
                    for (int i = 1; i < 4; i++) {
                        float p = Vector2.Dot(obb.Corners[i], ax);
                        if (p < min) min = p;
                        if (p > max) max = p;
                    }
                }

                Project(this, axis, out float minA, out float maxA);
                Project(obstacle, axis, out float minB, out float maxB);

                float overlap = Math.Min(maxA, maxB) - Math.Max(minA, minB);
                if (overlap <= 0)
                    return Vector2.Zero; // No collision

                if (overlap < minOverlap) {
                    minOverlap = overlap;
                    mtvAxis = axis;
                }
            }

            // Ensure MTV points away from moving object
            Vector2 direction = this.Center - obstacle.Center;
            if (Vector2.Dot(direction, mtvAxis) < 0)
                mtvAxis = -mtvAxis;

            return mtvAxis * minOverlap;
        }



        // Project corners onto axis and check overlap
        private bool OverlapOnAxis(OBB a, OBB b, Vector2 axis) {
                void Project(OBB obb, Vector2 ax, out float min, out float max) {
                    min = max = Vector2.Dot(obb.Corners[0], ax);
                    for (int i = 1; i < 4; i++) {
                        float p = Vector2.Dot(obb.Corners[i], ax);
                        if (p < min) min = p;
                        if (p > max) max = p;
                    }
                }

                Project(a, axis, out float minA, out float maxA);
                Project(b, axis, out float minB, out float maxB);

                return !(maxA < minB || maxB < minA);
            }
        }
    }

