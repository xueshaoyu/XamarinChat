using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Content.Core
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatPage : ContentPage
	{
		public ChatPage ()
		{
			InitializeComponent ();
		}

        /// <summary>
        /// jump to MainPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_MainPage(object sender, EventArgs e)
        {

        }
    }
}