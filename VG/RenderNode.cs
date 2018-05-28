using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PiGameSharp.VG
{
	/// <summary>
	/// Base class for nodes in the scene graph. Each node can have a parent and many children. A transformation update automatically propagates to the child nodes. 
	/// </summary>
	public class RenderNode
	{
		private RenderNode par;
		private List<RenderNode> children;
		private Matrix transf = Matrix.Identity;

		/// <summary>
		/// Gets or sets the parent of this node
		/// </summary>
		/// <value>The parent</value>
		public RenderNode Parent
		{
			get => par;
			set
			{
				if (par != null)
				{
					par.children.Remove(this);
					if (par.children.Count == 0)
						par.children = null;
				}
				par = value;
				foreach (RenderNode p in Parents)
					if (p == this)
					{
						par = null;
						throw new Exception("Unable to create ancestry loop");
					}
				if (par != null)
				{
					if (par.children == null)
						par.children = new List<RenderNode>();
					par.children.Add(this);
				}
				UpdateTransformation();
			}
		}

		/// <summary>
		/// Gets or sets the transformation of this node relative to its parent node, or the world if this node has no parent.
		/// </summary>
		/// <value>The transformation</value>
		public Matrix Transform
		{
			get => transf;
			set
			{
				transf = value;
				UpdateTransformation();
			}
		}

		/// <summary>
		/// Gets the world transformation of this node. That is, the transformation of all parents and this node expressed as a transformation.
		/// </summary>
		/// <value>The world transform.</value>
		public Matrix WorldTransform { get; private set; } = Matrix.Identity;

		/// <summary>
		/// Gets and enumerable for the ancestors of this node
		/// </summary>
		/// <value>The enumerable for the ancestors</value>
		public IEnumerable<RenderNode> Parents
		{
			get
			{
				RenderNode p = par;
				while (p != null)
				{
					yield return p;
					p = p.par;
				}
				yield break;
			}
		}

		/// <summary>
		/// Gets an enumerable for the children of this node
		/// </summary>
		/// <value>The enumerable for the children</value>
		public IEnumerable<RenderNode> Children
		{
			get
			{
				foreach (RenderNode r in children)
					yield return r;
				yield break;
			}
		}

		/// <summary>
		/// Draw this render node and its children
		/// </summary>
		/// <remarks>
		/// The default render node has no visual drawing, so it only propagates this call to it's children.
		/// </remarks>
		public virtual void Draw()
		{
			if (children != null)
				foreach (RenderNode rg in children)
					rg.Draw();
		}

		/// <summary>
		/// Draw the debug display of this render node and its children
		/// </summary>
		[Conditional("DEBUG")]
		public virtual void DrawDebug()
		{
			if (children != null)
				foreach (RenderNode rg in children)
					rg.DrawDebug();
		}


		public RenderNode this[int child] => children[child];

		private void UpdateTransformation()
		{
			if (par != null)
				WorldTransform = par.WorldTransform * transf;
			else
				WorldTransform = transf;
			if (children != null)
				foreach (RenderNode rg in children)
					rg.UpdateTransformation();
		}
	}
}
