﻿using OpenGL;
using System.Diagnostics;
using System.Numerics;
using Quaternion = System.Numerics.Quaternion;

namespace ChungusEngine
{
    public class Camera
    {
        private Vector3 PitchYawRoll = new();

        public Vector3 Position = new(0.0f, 0.0f, -5.0f);

        private Vector3 Scale = Static.Unit3;

        private Matrix4x4f CameraTransform { get { 
                return Matrix4x4f.Translated(Position.X, Position.Y, Position.Z) 
                    * Matrix4x4f.Scaled(Scale.X, Scale.Y, Scale.Z) 
                    * BuildRotationMatrix();
        } }

        public Matrix4x4f View()
        {
            return CameraTransform;
        }

        public Matrix4x4f Projection()
        {
            return Perspective();
        }

        public Vector3 GetForwardVector()
        {
            var m = CameraTransform.Inverse;
            return new Vector3(m.Column2.x, m.Column2.y, m.Column2.z).Normalize();
        }

        public Vector3 GetRightVector()
        {
            var m = CameraTransform.Inverse;
            return new Vector3(m.Column0.x, m.Column0.y, m.Column0.z).Normalize();
        }

        static Vertex3f Up = new(0.0f, 1.0f, 0.0f);

        public static Vertex3f GetUpVector() => Up;

        public Matrix4x4f Perspective() => Matrix4x4f.Perspective(45.0f, 800.0f / 600.0f, 1.0f, 100.0f);

        public void UpdateCameraRotation(Vector2 MouseVector)
        {
            float sensitivity = 0.05f;

            PitchYawRoll.X += MouseVector.Y * sensitivity;
            PitchYawRoll.Y += MouseVector.X * sensitivity;

            // Prevent the camera from flipping over when moving up and down
            // extension methods are so cool!!!!!!!!!
            PitchYawRoll.X = PitchYawRoll.X.Clamp(-90.0f, 90.0f);
        }

        private float speed = 100.0f;

        private void MoveForward()
        {
            Position += GetForwardVector() * speed * Static.XZPlane3 * DeltaTime.Dt;
        }

        private void MoveBackward()
        {
            Position -= GetForwardVector() * speed * Static.XZPlane3 * DeltaTime.Dt;
        }

        private void MoveRight()
        {
            Position -= GetRightVector() * speed * Static.XZPlane3 * DeltaTime.Dt;
        }

        private void MoveLeft()
        {
            Position += GetRightVector() * speed * Static.XZPlane3 * DeltaTime.Dt;
        }

        internal void HandleKeyboardInput(Keys code)
        {
            switch (code)
            {
                case Keys.W:
                    MoveForward();
                    break;
                case Keys.A:
                    MoveLeft();
                    break;
                case Keys.S:
                    MoveBackward();
                    break;
                case Keys.D:
                    MoveRight();
                    break;
            }
        }

        public Matrix4x4f BuildRotationMatrix() =>
            Matrix4x4f.RotatedZ(PitchYawRoll.Z) 
            * Matrix4x4f.RotatedY(PitchYawRoll.Y) 
            * Matrix4x4f.RotatedX(PitchYawRoll.X);

        internal Camera()
        {
        }
    }

}
