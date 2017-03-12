using Xamarin.Forms;
using System.Threading.Tasks;

namespace DesktopAppDemo
{
	public partial class DesktopAppDemoPage : ContentPage
	{
		void Handle_Clicked(object sender, System.EventArgs e)
		{
			Btn.Text = "Please wait...";
			DownloadFileAsync().Wait();
			Btn.Text = "File downloaded";
		}

		async Task DownloadFileAsync()
		{
			await Task.Delay(1000);
		}

		public DesktopAppDemoPage()
		{
			InitializeComponent();
		}
	}
}
