namespace ITmeze.Core.Views
{
	public interface IViewProjectionFactory
	{
		TOut Get<TIn, TOut>(TIn input);
	}
}