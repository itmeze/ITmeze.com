namespace ITmeze.Core.Views
{
	public interface IViewProjection<tIn, tOut>
	{
		tOut Project(tIn input);
	}
}
