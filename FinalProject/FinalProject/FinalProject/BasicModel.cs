﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    public class BasicModel
    {
        public Model Model { get; protected set; }

        BoundingSphere boundingSphere;

        public BasicModel(Model model)
        {
            this.Model = model;
            this.boundingSphere = GenerateBoundingSphere();
        }

        protected BoundingSphere GenerateBoundingSphere()
        {
            // Generate a bounding sphere that will encapsulate the entire model
            BoundingSphere boundingSphere = new BoundingSphere(Vector3.Zero, 0);

            // Merge all mesh bounding spheres into one
            foreach (ModelMesh mesh in Model.Meshes)
                boundingSphere = BoundingSphere.CreateMerged(boundingSphere, mesh.BoundingSphere);

            return boundingSphere;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw each mesh in the model
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect basicEffect in mesh.Effects)
                {
                    basicEffect.EnableDefaultLighting();

                    basicEffect.Projection = camera.Projection;
                    basicEffect.View = camera.View;

                    Matrix world = GetWorld(transforms[mesh.ParentBone.Index], camera);
                    basicEffect.World = world;
                }
                mesh.Draw();
            }
        }

        // Provide a way for subclasses to alter the world transformation of the model
        protected virtual Matrix GetWorld(Matrix meshTransform, Camera camera)
        {
            return Matrix.Identity * meshTransform;
        }

        // Provides rudimentary collision detection
        public virtual bool Collides(Vector3 point)
        {
            return boundingSphere.Contains(point) != ContainmentType.Disjoint;
        }

        // Use the god sphere that was generated by the constructor by default
        public virtual bool Collides(Model otherModel)
        {
            foreach (ModelMesh mesh in otherModel.Meshes)
            {
                if (mesh.BoundingSphere.Intersects(boundingSphere))
                    return true;
            }
            return false;   
        }
    }
}
