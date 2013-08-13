using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITmeze.Core.Views.Home
{
	public class HomeViewModel
	{
		public string Text = "Welcome";
	}

	public class HomeViewBindingModel
	{
		
	}

	public class HomeViewProjection : IViewProjection<HomeViewBindingModel, HomeViewModel>
	{
		public HomeViewModel Project(HomeViewBindingModel input)
		{
			return new HomeViewModel() {Text = "Buenas dias"};
		}
	}
}
