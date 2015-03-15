using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace RB3Tool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<int> charBasesA = new List<int>();
		List<int> charBasesB = new List<int>();

		public MainWindow()
		{
			InitializeComponent();

			charBasesA.Add(0x581A7);
			charBasesB.Add(0x4A21A7);
			charBasesA.Add(0x7909D);
			charBasesB.Add(0x4C509D);
			charBasesA.Add(0x99F93);
			charBasesB.Add(0x4E5F93);
			charBasesA.Add(0xBEE89);
			charBasesB.Add(0x506E89);
			charBasesA.Add(0xDFD7F);
			charBasesB.Add(0x527D7F);
			charBasesA.Add(0x100C75);
			charBasesB.Add(0x548C75);
			charBasesA.Add(0x121B6B);
			charBasesB.Add(0x569B6B);
			charBasesA.Add(0x142A5D);
			charBasesB.Add(0x58CA5D);
			charBasesA.Add(0x163957);
			charBasesB.Add(0x5AD957);
			charBasesA.Add(0x18684D);
			charBasesB.Add(0x5CE84D);
		}

		private void btnOpen_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Title = "Open Save";
            ofd.Filter = "band3 (*.*)|*band3*";
			//if (ofd.ShowDialog() == DialogResult)
			if ((bool)ofd.ShowDialog())
			{
				FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
				BinaryReader br = new BinaryReader(fs);
				fs.Position = 0;

				//Make sure this is a con file
				var fileMagic = br.ReadInt32();
				if (fileMagic != 542003011)
				{
					fs.Close();
					br.Close();
					MessageBox.Show("This file is not a CON container.");
					return;
				}
				charTabs.Items.Clear();
				txtFile.Text = ofd.FileName;
				for (int i = 0; i < 10; i++)
				{
					fs.Position = charBasesB[i];
					int namesize = br.ReadInt32();
					if (namesize > 0)
					{
						string name = Encoding.ASCII.GetString(br.ReadBytes(namesize));

						charTabs.Items.Add(new TabItem
						{
							Header = name,
							Content = new Character(ofd.FileName, fs, br, i, charBasesA, charBasesB),
						});
					}
				}
				fs.Close();
				br.Close();
				if (charTabs.Items.Count <= 0)
					MessageBox.Show("This save does not appear to contain any characters. You may need to modify them in-game first.");
				return;

			}
		}
		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			charTabs.Items.Clear();
			txtFile.Text = "No File Loaded.";
		}
	}
}
