using System;
using System.Collections.Generic;

namespace PiGameSharp.VG
{
	public class RenderGraph
	{
		public RenderGraph par;
		public List<RenderGraph> Children;
		private Matrix transf = Matrix.Identity;
		public Matrix CompoundedTransform = Matrix.Identity;
		
		public RenderGraph Parent
		{
			get
			{
				return par;
			}
			set
			{
				if (par != null)
				{
					par.Children.Remove(this);
					if (par.Children.Count == 0)
						par.Children = null;
				}
				par = value;
				foreach (RenderGraph p in Parents)
					if (p == this)
					{
						par = null;
						throw new Exception("Unable to create ancestry loop");
					}
				if (par != null)
				{
					if (par.Children == null)
						par.Children = new List<RenderGraph>();
					par.Children.Add(this);
				}
				UpdateTransformation();
			}
		}
		
		public Matrix Transform
		{
			get
			{
				return transf;
			}
			set
			{
				transf = value;
				UpdateTransformation();
			}
		}

		public IEnumerable<RenderGraph> Parents
		{
			get
			{
				RenderGraph p = par;
				while (p != null)
				{
					yield return p;
					p = p.par;
				}
				yield break;
			}
		}

		public virtual void Draw()
		{
			if (Children != null)
				foreach (RenderGraph rg in Children)
					rg.Draw();
		}

		private void UpdateTransformation()
		{
			if (par != null)
				CompoundedTransform = par.CompoundedTransform * this.transf;
			else
				CompoundedTransform = this.transf;
			if (Children != null)
				foreach (RenderGraph rg in Children)
					rg.UpdateTransformation();
		}
	}
}
