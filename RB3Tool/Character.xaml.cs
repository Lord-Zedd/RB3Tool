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
	/// Interaction logic for Character.xaml
	/// </summary>
	public partial class Character
	{
		string genderString;

		#region offsets
		private int Char_Gender = 0x94;
		/*
		 * x98 skin color
		 *     eye color
		 *     cheek
		 *     chin
		 *     chinw
		 *     chinv
		 *     cheekw
		 *     cheekv
		 *     nose
		 *     nosew
		 *     nosev
		 *     eyes
		 *     eyesa
		 *     eyesd
		 *     eyesv
		 *     mouth
		 *     mouthw
		 *     mouthv
		 *     browd
		 *     browv
		 *     brow string length
		 * xEC brow string
		 */
		private int Char_FaceHair = 0x164;
		private int Char_Glasses = 0x1A2;
		private int Char_Hair = 0x1E0;

		private int Char_Feet = 0x25C;
		private int Char_Hands = 0x29A;
		private int Char_Legs = 0x2D8;
		private int Char_Torso = 0x354;
		private int Char_Wrist = 0x392;
		private int Char_Height = 0x3D0;
		// x3D4 weight
		// x3D8 muscle, only 0-1 is valid for weight and muscle
		private int Char_Guitar = 0x3DC;
		private int Char_Bass = 0x41A;
		private int Char_Drum = 0x458;
		private int Char_Mic = 0x496;
		private int Char_Key = 0x4D4;
		#endregion

		string saveFile = "";
		int charIndex;

		List<int> charBasesA = new List<int>();
		List<int> charBasesB = new List<int>();

		public Character(string fileName, FileStream fs, BinaryReader br, int index, List<int> charBaseA, List<int> charBaseB)
		{
			int currentChar = charBaseA[index];
			
			// store stuff from mainwindow
			charIndex = index;
			saveFile = fileName;
			charBasesA = charBaseA;
			charBasesB = charBaseB;

			// check initial gender to load face images
			fs.Position = currentChar + Char_Gender;
			if (br.ReadInt32() == 2)
				genderString = "female";
			else
				genderString = "male";

			InitializeComponent();

			ReadCharacter(br, fs);
		}

		private void charReload_Click(object sender, RoutedEventArgs e)
		{
			if (!File.Exists(saveFile))
			{
				MessageBox.Show("The loaded save file could not be found. Check the file and try again.");
				return;
			}

			FileStream fs = new FileStream(saveFile, FileMode.Open, FileAccess.Read);
			BinaryReader br = new BinaryReader(fs);

			ReadCharacter(br, fs);

			fs.Close();
			br.Close();
		}

		private void ReadCharacter(BinaryReader br, FileStream fs)
		{
			int baseOffset = charBasesA[charIndex];
			var stringLength = 0;

			fs.Position = baseOffset;
			stringLength = br.ReadInt32();
			charName.Text = Encoding.ASCII.GetString(br.ReadBytes(stringLength));
			fs.Position = baseOffset + Char_Gender;
			charGender.SelectedIndex = (br.ReadInt32() - 2);
			charSkinColor.SelectedIndex = br.ReadInt32();
			charEyeColor.SelectedIndex = br.ReadInt32();

			PopulateCombos();
			AddGuitar();
			AddBass();
			AddDrum();
			AddMic();
			AddKeys();

			charCheek.Value = (br.ReadInt32() + 1);
			charChin.Value = (br.ReadInt32() + 1);
			charChinW.Value = br.ReadSingle();
			charChinV.Value = br.ReadSingle();
			charCheekW.Value = br.ReadSingle();
			charCheekV.Value = br.ReadSingle();

			charNose.Value = (br.ReadInt32() + 1);
			charNoseW.Value = br.ReadSingle();
			charNoseV.Value = br.ReadSingle();

			charEye.Value = (br.ReadInt32() + 1);
			charEyeA.Value = br.ReadSingle();
			charEyeD.Value = br.ReadSingle();
			charEyeV.Value = br.ReadSingle();

			charMouth.Value = (br.ReadInt32() + 1);
			charMouthW.Value = br.ReadSingle();
			charMouthV.Value = br.ReadSingle();
			charBrowD.Value = br.ReadSingle();
			charBrowV.Value = br.ReadSingle();

			fs.Position = baseOffset + Char_Height;
			charHeight.Value = br.ReadSingle();
			charWeight.Value = br.ReadSingle();
			charMuscle.Value = br.ReadSingle();


			fs.Position = baseOffset + Char_Feet;
			ReadRBString(br, 1, charFeet);

			fs.Position = baseOffset + Char_Hands;
			ReadRBString(br, 1, charHand);

			fs.Position = baseOffset + Char_Legs;
			ReadRBString(br, 1, charLeg);

			fs.Position = baseOffset + Char_Torso;
			ReadRBString(br, 1, charTorso);

			fs.Position = baseOffset + Char_Guitar;
			ReadRBString(br, 0, charGuitar);

			fs.Position = baseOffset + Char_Bass;
			ReadRBString(br, 0, charBass);

			fs.Position = baseOffset + Char_Drum;
			ReadRBString(br, 0, charDrum);

			fs.Position = baseOffset + Char_Mic;
			ReadRBString(br, 0, charMic);

			fs.Position = baseOffset + Char_Key;
			ReadRBString(br, 0, charKeys);

			fs.Position = baseOffset + Char_FaceHair;
			ReadRBString(br, 0, charFaceHair);

			fs.Position = baseOffset + Char_Glasses;
			ReadRBString(br, 0, charGlasses);

			fs.Position = baseOffset + Char_Hair;
			ReadRBString(br, 0, charHair);

			fs.Position = baseOffset + Char_Wrist;
			ReadRBString(br, 0, charWrist);
		}

		private void ReadRBString2(BinaryReader br, int fallbackIndex, ComboBox box)
		{
			var stringLength = 0;

			stringLength = br.ReadInt32();
			box.SelectedItem = Encoding.ASCII.GetString(br.ReadBytes(stringLength));
			if (box.SelectedIndex == -1)
				box.SelectedIndex = fallbackIndex;
		}

		private void ReadRBString(BinaryReader br, int fallbackIndex, ComboBox box)
		{
			var stringLength = 0;
			stringLength = br.ReadInt32();

			RBString loadString = new RBString(Encoding.ASCII.GetString(br.ReadBytes(stringLength)), null);
			var blah = box.Items.IndexOf(loadString);
			box.SelectedIndex = blah;

			if (box.SelectedIndex == -1)
				box.SelectedIndex = fallbackIndex;
		}

		private void PopulateCombos()
		{
			charFeet.Items.Clear();
			charHand.Items.Clear();
			charLeg.Items.Clear();
			charTorso.Items.Clear();

			charFaceHair.Items.Clear();
			charGlasses.Items.Clear();
			charHair.Items.Clear();
			charWrist.Items.Clear();

			if (charGender.SelectedIndex == 0)
			{
				AddFemFeet();
				AddFemHand();
				AddFemLeg();
				AddFemTorso();

				AddFemFaceHair();
				AddFemHair();
				AddFemWrist();
			}
			else
			{
				AddMaleFeet();
				AddMaleHand();
				AddMaleLeg();
				AddMaleTorso();

				AddMaleFaceHair();
				AddMaleHair();
				AddMaleWrist();
			}
			AddGlasses();
		}

		private void charGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (charGender.SelectedIndex == 0)
				genderString = "female";
			else
				genderString = "male";

			// load new combos and reset to naked except glasses
			var glasses = charGlasses.SelectedIndex;
			PopulateCombos();
			charFeet.SelectedIndex = 1;
			charHand.SelectedIndex = 1;
			charLeg.SelectedIndex = 1;
			charTorso.SelectedIndex = 1;

			charFaceHair.SelectedIndex = 0;
			charGlasses.SelectedIndex = glasses;
			charHair.SelectedIndex = 0;
			charWrist.SelectedIndex = 0;

			// update face images
			charCheekImg.Source = new BitmapImage(setFaceImageUri(charCheek.Value.Value - 1, "_shape_"));
			charChinImg.Source = new BitmapImage(setFaceImageUri(charChin.Value.Value - 1, "_chin_"));
			charNoseImg.Source = new BitmapImage(setFaceImageUri(charNose.Value.Value - 1, "_nose_"));
			charEyeImg.Source = new BitmapImage(setFaceImageUri(charEye.Value.Value - 1, "_eye_"));
			charMouthImg.Source = new BitmapImage(setFaceImageUri(charMouth.Value.Value - 1, "_mouth_"));
		}

		private void charSave_Click(object sender, RoutedEventArgs e)
		{
			int offsetA = charBasesA[charIndex];
			int offsetB = charBasesB[charIndex];

			if (!File.Exists(saveFile))
			{
				MessageBox.Show("The loaded save file could not be found. Check the file and try again.");
				return;
			}

			FileStream fs = new FileStream(saveFile, FileMode.Open, FileAccess.Write);
			BinaryWriter bw = new BinaryWriter(fs);

			WriteCharacter(offsetA, fs, bw);
			WriteCharacter(offsetB, fs, bw);

			fs.Close();
			bw.Close();
			MessageBox.Show("Character saved successfully! Don't forget to rehash/resign.");
		}

		private void WriteCharacter(int baseOffset, FileStream fs, BinaryWriter bw)
		{
			fs.Position = baseOffset;
			bw.Write(charName.Text.Length);
			bw.Write(Encoding.ASCII.GetBytes(charName.Text));

			fs.Position = baseOffset + Char_Gender;
			bw.Write(charGender.SelectedIndex + 2);
			bw.Write(charSkinColor.SelectedIndex);
			bw.Write(charEyeColor.SelectedIndex);

			bw.Write(charCheek.Value.Value - 1);
			bw.Write(charChin.Value.Value - 1);
			bw.Write(charChinW.Value.Value);
			bw.Write(charChinV.Value.Value);
			bw.Write(charCheekW.Value.Value);
			bw.Write(charCheekV.Value.Value);

			bw.Write(charNose.Value.Value - 1);
			bw.Write(charNoseW.Value.Value);
			bw.Write(charNoseV.Value.Value);

			bw.Write(charEye.Value.Value - 1);
			bw.Write(charEyeA.Value.Value);
			bw.Write(charEyeD.Value.Value);
			bw.Write(charEyeV.Value.Value);

			bw.Write(charMouth.Value.Value - 1);
			bw.Write(charMouthW.Value.Value);
			bw.Write(charMouthV.Value.Value);
			bw.Write(charBrowD.Value.Value);
			bw.Write(charBrowV.Value.Value);

			fs.Position = baseOffset + Char_Height;
			bw.Write(charHeight.Value.Value);
			bw.Write(charWeight.Value.Value);
			bw.Write(charMuscle.Value.Value);

			fs.Position = baseOffset + Char_Feet;
			WriteRBString(bw, charFeet);

			fs.Position = baseOffset + Char_Hands;
			WriteRBString(bw, charHand);

			fs.Position = baseOffset + Char_Legs;
			WriteRBString(bw, charLeg);

			fs.Position = baseOffset + Char_Torso;
			WriteRBString(bw, charTorso);

			fs.Position = baseOffset + Char_Guitar;
			WriteRBString(bw, charGuitar);

			fs.Position = baseOffset + Char_Bass;
			WriteRBString(bw, charBass);

			fs.Position = baseOffset + Char_Drum;
			WriteRBString(bw, charDrum);

			fs.Position = baseOffset + Char_Mic;
			WriteRBString(bw, charMic);

			fs.Position = baseOffset + Char_Key;
			WriteRBString(bw, charKeys);

			fs.Position = baseOffset + Char_FaceHair;
			WriteRBString(bw, charFaceHair);

			fs.Position = baseOffset + Char_Glasses;
			WriteRBString(bw, charGlasses);

			fs.Position = baseOffset + Char_Hair;
			WriteRBString(bw, charHair);

			fs.Position = baseOffset + Char_Wrist;
			WriteRBString(bw, charWrist);
		}

		private void WriteRBString(BinaryWriter bw, ComboBox box)
		{
			RBString item = (RBString)box.SelectedItem;

			bw.Write(item.saveString.Length);
			bw.Write(Encoding.ASCII.GetBytes(item.saveString));
		}

		#region combobox helpers
		private void AddFemFeet()
		{
			charFeet.Items.Add(new RBString("", "Null (Invisible)"));
			charFeet.Items.Add(new RBString("female_feet_naked", "Barefoot"));
			charFeet.Items.Add(new RBString("3strapheels_patenleather", "Lenore's Heels"));
			charFeet.Items.Add(new RBString("aztecboots_hide", "Toci's Feet"));
			charFeet.Items.Add(new RBString("batheel_metal", "Bat Out of Heels"));
			charFeet.Items.Add(new RBString("berzerkerboot_spiked", "Stompinwolf"));
			charFeet.Items.Add(new RBString("chucktaylor_folded", "Folded Dornholes"));
			charFeet.Items.Add(new RBString("chunkyboots_plaid", "Plaid Reputation Boots"));
			charFeet.Items.Add(new RBString("combatbootsmetal_armored", "Butane Jane"));
			charFeet.Items.Add(new RBString("comboshoes_twotone", "Slouchy Stirrup Boots"));
			charFeet.Items.Add(new RBString("creepersneakers_plaid", "Brawler Crawlers"));
			charFeet.Items.Add(new RBString("demonboots_stone", "Stone Demons"));
			charFeet.Items.Add(new RBString("docsductape_leather", "Tape Heads"));
			charFeet.Items.Add(new RBString("dragonslayerboots_scales", "Wyvern Slayer Boots"));
			charFeet.Items.Add(new RBString("femalecombatboots_leather", "Combat Boots"));
			charFeet.Items.Add(new RBString("femalecombatboots_var1", "Painters"));
			charFeet.Items.Add(new RBString("femalecombatboots_var2", "Color Me Plaid"));
			charFeet.Items.Add(new RBString("femalecowboyboots_embroidered", "Embroidered Western Boots"));
			charFeet.Items.Add(new RBString("femalecowboyboots_sunburst", "Ghost Ranchers"));
			charFeet.Items.Add(new RBString("femalecowboyboots_western", "Western Boots"));
			charFeet.Items.Add(new RBString("femaledestroyedchucks_solid", "Worn Dornholes"));
			charFeet.Items.Add(new RBString("femaledestroyedchucks_starsandstripes", "Stars & Hype"));
			charFeet.Items.Add(new RBString("femaledestroyedchucks_var1", "Dirty Mouth Breathers"));
			charFeet.Items.Add(new RBString("femaledestroyedchucks_var2", "Dee-lite Dornholes"));
			charFeet.Items.Add(new RBString("femaleeightholedocs_checkered", "Checkered Past Boots"));
			charFeet.Items.Add(new RBString("femaleeightholedocs_unionjack", "Flag Day Boots"));
			charFeet.Items.Add(new RBString("femaleeightholedocs_var1", "Lace Pacers"));
			charFeet.Items.Add(new RBString("femaleeightholedocs_var2", "Splatter 6s"));
			charFeet.Items.Add(new RBString("femaleeightholedocs_var3", "Smoothies"));
			charFeet.Items.Add(new RBString("femalefurboots_leather", "Bont Laarzen"));
			charFeet.Items.Add(new RBString("femalegothstompers_leather", "Visigothic Treaders"));
			charFeet.Items.Add(new RBString("femalehighspikeboots_leather", "War Club Boots"));
			charFeet.Items.Add(new RBString("femalelowchucks_skullsprint", "Skully Dornholes"));
			charFeet.Items.Add(new RBString("femalelowchucks_sneakerskullsargyle", "Skullduggery Sneakers"));
			charFeet.Items.Add(new RBString("femalelowchucks_sneakertigerprint", "Prisoner of Roar Sneakers"));
			charFeet.Items.Add(new RBString("femalelowchucks_striped", "Striped Dornholes"));
			charFeet.Items.Add(new RBString("femalemotorcycleboots_solid", "Devil's Angels"));
			charFeet.Items.Add(new RBString("femaleplateboots_leather", "Plate Metal Boots"));
			charFeet.Items.Add(new RBString("femalerinoboots_leather", "Tricerabottoms"));
			charFeet.Items.Add(new RBString("femaleroadwarriorboots_leather", "Thunder Dome Power Stompers"));
			charFeet.Items.Add(new RBString("femalesambas_leather", "Marimbas"));
			charFeet.Items.Add(new RBString("femalesneakersstripes_nylon", "Mercuries"));
			charFeet.Items.Add(new RBString("femalesportysneaks_pleather", "Basketball Trainers"));
			charFeet.Items.Add(new RBString("femalestuddedcowboyboots_leather", "Nightmare Cowboy"));
			charFeet.Items.Add(new RBString("femgothslippers_satin", "Mourning Glory Slippers"));
			charFeet.Items.Add(new RBString("femmetalboots_leather", "Venom Vixen Boots"));
			charFeet.Items.Add(new RBString("femplateboots_dirty", "Metal Plates"));
			charFeet.Items.Add(new RBString("fempunkfeet_tape", "Punks Not Threads Feet"));
			charFeet.Items.Add(new RBString("femrockboots_leather", "Worn in the USA Boots"));
			charFeet.Items.Add(new RBString("flatslips_stripe", "The Wide Stripes"));
			charFeet.Items.Add(new RBString("gearheadboots_leather", "Roadstar Boots"));
			charFeet.Items.Add(new RBString("giantkillerboots_metal", "Giant Killer Boots"));
			charFeet.Items.Add(new RBString("gogoboot_tiger", "Go Tiger!"));
			charFeet.Items.Add(new RBString("highheels_solid", "9-to-5 Heels"));
			charFeet.Items.Add(new RBString("highstraps_solid", "Strap and Spike Heels"));
			charFeet.Items.Add(new RBString("ironmaidenboots_metal", "Titanium Sabatons"));
			charFeet.Items.Add(new RBString("kneehighdocsfemale_leather", "Goody Two Tone Boots"));
			charFeet.Items.Add(new RBString("kneehighheel_leather", "Bela Boots"));
			charFeet.Items.Add(new RBString("kneehighlaceup_leather", "Which Witch's Boots"));
			charFeet.Items.Add(new RBString("kneehighplatform_pvc", "Safety Straps"));
			charFeet.Items.Add(new RBString("lowtopsneaks_skater", "Low Skool Kicks"));
			charFeet.Items.Add(new RBString("maryjanes_retro", "Mary Birds"));
			charFeet.Items.Add(new RBString("midcalfplatform_leather", "Stand & Deliver Platforms"));
			charFeet.Items.Add(new RBString("midcalfspikes_leather", "Treacherous Spikes"));
			charFeet.Items.Add(new RBString("modavengerboots_leather", "Kinky Boots"));
			charFeet.Items.Add(new RBString("oilspillboots_latex", "Citlalicue's Feet"));
			charFeet.Items.Add(new RBString("opentoe_leather", "Sickness and Pedicure Heels"));
			charFeet.Items.Add(new RBString("plasticheels_pvc", "Nine Inch Heels"));
			charFeet.Items.Add(new RBString("polyfurboots_plain", "Clydesdales"));
			charFeet.Items.Add(new RBString("rennboots_leather", "Maid Marion Boots"));
			charFeet.Items.Add(new RBString("rubberboots_solid", "Wellies"));
			charFeet.Items.Add(new RBString("saddleanklesocks_ankle", "Back in the Saddle w/ Ankle Socks"));
			charFeet.Items.Add(new RBString("saddlekneesocks_argyle", "Back in the Saddle w/ Knee Socks"));
			charFeet.Items.Add(new RBString("saddleshoes_leather", "Back in the Saddle Shoes"));
			charFeet.Items.Add(new RBString("saggydocs_var1", "Electro-tones"));
			charFeet.Items.Add(new RBString("saggydocs_var2", "Run Down"));
			charFeet.Items.Add(new RBString("saggydocs_worn", "12-Hole Red Riders"));
			charFeet.Items.Add(new RBString("semispats_solid", "Motorcycle Mama"));
			charFeet.Items.Add(new RBString("shortrainboots_whitevinyl", "When It Rains Boots"));
			charFeet.Items.Add(new RBString("skates_dirty", "Bipolar Skates"));
			charFeet.Items.Add(new RBString("slipons_checkerboard", "Skandalous Slip-ons"));
			charFeet.Items.Add(new RBString("slouchyboots_solid", "Radically '80s Boots"));
			charFeet.Items.Add(new RBString("sneakerboots_vinyl", "Sneaker Boots"));
			charFeet.Items.Add(new RBString("spikedboots_leather", "Spiketress Boots"));
			charFeet.Items.Add(new RBString("spikedplatformboots_solid", "Platform Anklysaurs"));
			charFeet.Items.Add(new RBString("spikydocs_solid", "Motorcycle Mayhem Boots"));
			charFeet.Items.Add(new RBString("spinyboots_iron", "Chills Up Your Spine Boots"));
			charFeet.Items.Add(new RBString("spurboots_leather", "Venus in Spurs"));
			charFeet.Items.Add(new RBString("studdedshoe_leather", "Understudy Boots"));
			charFeet.Items.Add(new RBString("superhightops_leopard", "Leopard Skin Ultratops"));
			charFeet.Items.Add(new RBString("tallbuttonboots_embroidered", "Lyrical Swans"));
			charFeet.Items.Add(new RBString("talldocsfolded_solid", "Infinite Boots"));
			charFeet.Items.Add(new RBString("tallfoldedboots_solid", "Walkin' Boots"));
			charFeet.Items.Add(new RBString("tallgogos_vinyl", "Wake Me Up Before You Go Gos"));
			charFeet.Items.Add(new RBString("tatteredshoe_dirty", "Teenage Kicks"));
			charFeet.Items.Add(new RBString("thighboots_leather", "Sky High Thigh Highs"));
			charFeet.Items.Add(new RBString("thighhighheel_pvc", "Mistress Stilettos"));
			charFeet.Items.Add(new RBString("tiedtallboots_suede", "Warrior Princess"));
			charFeet.Items.Add(new RBString("untiedankleboots_leather", "Basket Case Boots"));
			charFeet.Items.Add(new RBString("wrestlingboots_twotone", "Luchador Boots"));
		}
		private void AddMaleFeet()
		{
			charFeet.Items.Add(new RBString("", "Null (Invisible)"));
			charFeet.Items.Add(new RBString("male_feet_naked", "Barefoot"));
			charFeet.Items.Add(new RBString("basketballsneaks_custom", "Custom Kicks"));
			charFeet.Items.Add(new RBString("beatleboots_leather", "Scouser Boots"));
			charFeet.Items.Add(new RBString("boneboots_leather", "Bone Kickers"));
			charFeet.Items.Add(new RBString("bootswithinboots_leather", "Boot Lined Boots"));
			charFeet.Items.Add(new RBString("bootswithlaces_leather", "Future Perfect"));
			charFeet.Items.Add(new RBString("combatbootschain_canvas", "Chain-Link Boots"));
			charFeet.Items.Add(new RBString("combatboots_leather", "Crap Kickers"));
			charFeet.Items.Add(new RBString("combatboots_var1", "Painters"));
			charFeet.Items.Add(new RBString("combatboots_var2", "Color Me Plaid"));
			charFeet.Items.Add(new RBString("commandoboots_leather", "Das Boots"));
			charFeet.Items.Add(new RBString("cowboyboots_embroidered", "Embroidered Western Boots"));
			charFeet.Items.Add(new RBString("cowboyboots_sunburst", "Ghost Ranchers"));
			charFeet.Items.Add(new RBString("cowboyboots_western", "Western Boots"));
			charFeet.Items.Add(new RBString("creepers_leopardspots", "Big Cat Crawlers"));
			charFeet.Items.Add(new RBString("creepers_vintage", "Vintage Crawlers"));
			charFeet.Items.Add(new RBString("dappershoes_solidshiny", "Italian Half-Heeled Shoes"));
			charFeet.Items.Add(new RBString("destroyedchucks_solid", "Worn Dornholes"));
			charFeet.Items.Add(new RBString("destroyedchucks_starsandstripes", "Stars & Hype"));
			charFeet.Items.Add(new RBString("destroyedchucks_var1", "Dirty Mouth Breathers"));
			charFeet.Items.Add(new RBString("destroyedchucks_var2", "Dee-lites"));
			charFeet.Items.Add(new RBString("dressshoes_leather", "Velouraptors"));
			charFeet.Items.Add(new RBString("eightholedocs_checkered", "Checkered Past Boots"));
			charFeet.Items.Add(new RBString("eightholedocs_unionjack", "Flag Day Boots"));
			charFeet.Items.Add(new RBString("eightholedocs_var1", "Lace Pacers"));
			charFeet.Items.Add(new RBString("eightholedocs_var2", "Splatter 6s"));
			charFeet.Items.Add(new RBString("eightholedocs_var3", "Smoothies"));
			charFeet.Items.Add(new RBString("fatlaceskatersneakers_solid", "Puff Streets"));
			charFeet.Items.Add(new RBString("flameshoes_leather", "Burn Higher Boots"));
			charFeet.Items.Add(new RBString("fringeboots_leather", "Texas Two-Steppers"));
			charFeet.Items.Add(new RBString("furboots_leather", "Bont Laarzen"));
			charFeet.Items.Add(new RBString("gladiatorboots_leather", "American Heroes"));
			charFeet.Items.Add(new RBString("glamitardslippers_mirrorball", "Eternal Glamnation Slippers"));
			charFeet.Items.Add(new RBString("glamitardslippers_wizard", "Slippers of the Magic-User"));
			charFeet.Items.Add(new RBString("gothstompers_leather", "Visigothic Treaders"));
			charFeet.Items.Add(new RBString("gothtops_darkleather", "Feelin' Low Hightops"));
			charFeet.Items.Add(new RBString("halfboots_solidshiny", "Teddy Boy Boots"));
			charFeet.Items.Add(new RBString("highchucks_solid", "Hightop Dornholes"));
			charFeet.Items.Add(new RBString("highspikeboots_leather", "War Club Boots"));
			charFeet.Items.Add(new RBString("jackboots_canvas", "Buckle Down Boots"));
			charFeet.Items.Add(new RBString("kissboots_sparkles", "Dressed to Kill Boots"));
			charFeet.Items.Add(new RBString("kneehighdocsmale_leather", "22-Hole Glossy Leather Boots"));
			charFeet.Items.Add(new RBString("lacewrappedboots_wornleather", "Mummy Stompers"));
			charFeet.Items.Add(new RBString("linkboots_softleather", "Melancholy Calf Embracers"));
			charFeet.Items.Add(new RBString("lowchucks_skullprint", "Skully Dornholes"));
			charFeet.Items.Add(new RBString("lowchucks_sneakerskullsargyle", "Skullduggery Sneakers"));
			charFeet.Items.Add(new RBString("lowchucks_sneakertigerprint", "Prisoner of Roar Sneakers"));
			charFeet.Items.Add(new RBString("lowchucks_striped", "Striped Dornholes"));
			charFeet.Items.Add(new RBString("lowtopstompers_leather", "Things"));
			charFeet.Items.Add(new RBString("madmaxboots_plated", "Main Force Patrol Boots"));
			charFeet.Items.Add(new RBString("maleberzerkerboot_spiked", "Stompinwolf"));
			charFeet.Items.Add(new RBString("malechucktaylors_folded", "Folded Dornholes"));
			charFeet.Items.Add(new RBString("malecombatbootsmetal_armor", "Butane Joe"));
			charFeet.Items.Add(new RBString("maledocsductape_leather", "Tape Heads"));
			charFeet.Items.Add(new RBString("malegearheadboots_leather", "Roadstar Boots"));
			charFeet.Items.Add(new RBString("malegothboots_steel", "Kill'em Shakespeare Boots"));
			charFeet.Items.Add(new RBString("malehighheels_snakeskin", "I Would Die 4 Shoes"));
			charFeet.Items.Add(new RBString("malelowtopsneaks_skater", "Low Skool Kicks"));
			charFeet.Items.Add(new RBString("malemetalboots_hide", "Beast of Burden Boots"));
			charFeet.Items.Add(new RBString("malerockboots_rhinestone", "Rhinestone Cowboy Boots"));
			charFeet.Items.Add(new RBString("malesaggydocs_var1", "Electro-tones"));
			charFeet.Items.Add(new RBString("malesaggydocs_var2", "Run Down Boots"));
			charFeet.Items.Add(new RBString("malesaggydocs_worn", "12-Hole Red Riders"));
			charFeet.Items.Add(new RBString("maleslipons2_checkerboard", "Skandalous Slip-ons"));
			charFeet.Items.Add(new RBString("maleslipons_skully", "Self-Inflicted Slip-ons"));
			charFeet.Items.Add(new RBString("malespikydocs_solid", "Motorcycle Mayhem Boots"));
			charFeet.Items.Add(new RBString("malewrestlingboots_twotone", "Luchador Boots"));
			charFeet.Items.Add(new RBString("motorcycleboots_solid", "Devil's Angels"));
			charFeet.Items.Add(new RBString("nailboots_rusty", "Nailed to the Floor"));
			charFeet.Items.Add(new RBString("namboots_jungle", "Concrete Jungle Kickers"));
			charFeet.Items.Add(new RBString("perferatedshoes_metallic", "Tin Men"));
			charFeet.Items.Add(new RBString("pirateboots_leather", "Buccaneer Booties"));
			charFeet.Items.Add(new RBString("plateboots_leather", "Plate Metal Boots"));
			charFeet.Items.Add(new RBString("quadstrapboots_flaming", "Thunder Dome Battle Boots"));
			charFeet.Items.Add(new RBString("quadstrapboots_gothstompers", "Detonators"));
			charFeet.Items.Add(new RBString("ringhalfboots_worn", "Fab Harness Boots"));
			charFeet.Items.Add(new RBString("rinoboots_leather", "Tricerabottoms"));
			charFeet.Items.Add(new RBString("roadwarriorboots_leather", "Thunder Dome Power Stompers"));
			charFeet.Items.Add(new RBString("sambas_leather", "Marimbas"));
			charFeet.Items.Add(new RBString("sneakersbasketball_pleather", "Star Jumps"));
			charFeet.Items.Add(new RBString("sneakersbasketball_sporty", "Sporty Sneakers"));
			charFeet.Items.Add(new RBString("sneakersstripes_nylon", "Mercuries"));
			charFeet.Items.Add(new RBString("sneakerswrestling_canvas", "No. Thirteens"));
			charFeet.Items.Add(new RBString("sneakers_leather", "Italian Sneakers"));
			charFeet.Items.Add(new RBString("spats_twotone", "Jack Spats"));
			charFeet.Items.Add(new RBString("spikedarmorboots_leather", "Knights of the Metal Republic"));
			charFeet.Items.Add(new RBString("spikedshoes_leather", "Mighty Spikes"));
			charFeet.Items.Add(new RBString("sportbikeboots_leather", "Hanzo Boots"));
			charFeet.Items.Add(new RBString("sportysneaks_pleather", "Basketball Trainers"));
			charFeet.Items.Add(new RBString("stompershoes_leather", "Tommy Stompers"));
			charFeet.Items.Add(new RBString("studdedcowboyboots_leather", "Nightmare Cowboy"));
			charFeet.Items.Add(new RBString("summonershoes_burlap", "Summoner's Shoes"));
			charFeet.Items.Add(new RBString("tallboots_solid", "Combat Boots"));
			charFeet.Items.Add(new RBString("tatteredshoe_homeless", "Hobo Hightops"));
			charFeet.Items.Add(new RBString("timberlandboots_muddy", "Nubucks"));
			charFeet.Items.Add(new RBString("warlockboots_softleather", "Warlock Slippers"));
			charFeet.Items.Add(new RBString("westernslipons_embroidered", "Wango Tango Shoes"));
			charFeet.Items.Add(new RBString("wovensteppers_blackleather", "Dr. Acula Orthopedics"));
		}
		private void AddFemHand()
		{
			charHand.Items.Add(new RBString("", "Null (Invisible)"));
			charHand.Items.Add(new RBString("female_hands_naked", "Bare"));
			charHand.Items.Add(new RBString("female_hands_bandage_wrap", "Mummy Hand Wraps"));
			charHand.Items.Add(new RBString("female_hands_drivinggloves_leather", "Driving Gloves"));
			charHand.Items.Add(new RBString("female_hands_fingergloves_leather", "Finger Gloves"));
			charHand.Items.Add(new RBString("female_hands_fingerless_leather", "Palm Warmers"));
			charHand.Items.Add(new RBString("female_hands_fishnetgloves_fishnet", "Material Gloves"));
			charHand.Items.Add(new RBString("female_hands_glove_cotton", "Cotton Gloves"));
			charHand.Items.Add(new RBString("female_hands_hobogloves_cotton", "Hands in Gloves"));
			charHand.Items.Add(new RBString("female_hands_kidgloves_leather", "Kid Gloves"));
			charHand.Items.Add(new RBString("female_hands_long_leather", "Whole Lotta Gloves"));
			charHand.Items.Add(new RBString("female_hands_skeletongloves_skeleton", "Skelly Gloves"));
		}
		private void AddMaleHand()
		{
			charHand.Items.Add(new RBString("", "Null (Invisible)"));
			charHand.Items.Add(new RBString("male_hands_naked", "Bare"));
			charHand.Items.Add(new RBString("male_hands_bandage_wrap", "Mummy Hand Wraps"));
			charHand.Items.Add(new RBString("male_hands_fingerlessdriving", "Driving Gloves"));
			charHand.Items.Add(new RBString("male_hands_fingerless_leather", "Palm Warmers"));
			charHand.Items.Add(new RBString("male_hands_gloves_cotton", "Cotton Gloves"));
			charHand.Items.Add(new RBString("male_hands_hobogloves_cotton", "Hands in Gloves"));
			charHand.Items.Add(new RBString("male_hands_leathergloves_leather", "Whole Lotta Gloves"));
			charHand.Items.Add(new RBString("male_hands_skeletongloves_skeleton", "Skelly Gloves"));
		}
		private void AddFemLeg()
		{
			charLeg.Items.Add(new RBString("", "Null (Invisible)"));
			charLeg.Items.Add(new RBString("female_legs_naked", "Nude"));
			charLeg.Items.Add(new RBString("armorleggings_nylon", "Poleyn Leggings"));
			charLeg.Items.Add(new RBString("armymini_denim", "Sgt. Sexy"));
			charLeg.Items.Add(new RBString("aztecleggings_hide", "Toci's Legs"));
			charLeg.Items.Add(new RBString("barbedwirepants_denim", "Twisted Barbs"));
			charLeg.Items.Add(new RBString("batbeltkneepadpants_denim", "Belfrey Denim"));
			charLeg.Items.Add(new RBString("bellbottoms_jeans", "Swell Bottoms"));
			charLeg.Items.Add(new RBString("beltskirt_denim", "Jean Genie"));
			charLeg.Items.Add(new RBString("bikinistockings_fishnet", "No Regrets Fishnets"));
			charLeg.Items.Add(new RBString("bondagepants_plaidknees", "Trussed Issues"));
			charLeg.Items.Add(new RBString("breeches_wool", "Big For Your Breeches"));
			charLeg.Items.Add(new RBString("bulkyshorts_camo", "Hardcore Shorts"));
			charLeg.Items.Add(new RBString("bulkyshorts_cotton", "Bulky Shorts"));
			charLeg.Items.Add(new RBString("bulletbeltleggings_giraffe", "Gam Crackers"));
			charLeg.Items.Add(new RBString("bulletbeltleggings_rubber", "Josie Wails Leggings"));
			charLeg.Items.Add(new RBString("buttflappants_khaki", "Puck Tails"));
			charLeg.Items.Add(new RBString("capripants_tigerstripe", "Tiger Tiger"));
			charLeg.Items.Add(new RBString("cargopants_camo", "Camo Cargos"));
			charLeg.Items.Add(new RBString("clamdiggers_jeans", "Oi! Jeans!"));
			charLeg.Items.Add(new RBString("cleanminiskirt_solid", "Molly's Mini"));
			charLeg.Items.Add(new RBString("darkpleatedskirt_vinyl", "Vinylation"));
			charLeg.Items.Add(new RBString("demonleggings_lace", "Demons & Lace Leggings"));
			charLeg.Items.Add(new RBString("dragonslayerleggings_scales", "Wyvern Slayer Leggings"));
			charLeg.Items.Add(new RBString("dresspants_solid", "9-to-5 Pants"));
			charLeg.Items.Add(new RBString("feathercorsetbottom_bottom", "Par for the Corset Bottoms"));
			charLeg.Items.Add(new RBString("femalecargopants_canvas", "Jaunt Cargos"));
			charLeg.Items.Add(new RBString("femaleundies_cotton", "Brief Examples"));
			charLeg.Items.Add(new RBString("fembulletbriefs_silk", "Vigilante Panties"));
			charLeg.Items.Add(new RBString("femcords_worn", "Four Cords"));
			charLeg.Items.Add(new RBString("femgothskirt_satin", "Mourning Glory Skirt"));
			charLeg.Items.Add(new RBString("femmetaltights_spandex", "Venom Vixen Leggings"));
			charLeg.Items.Add(new RBString("fempunkbriefs_tape", "Punks Not Threads Briefs"));
			charLeg.Items.Add(new RBString("femrockchaps_leather", "Worn in the USA Chaps"));
			charLeg.Items.Add(new RBString("flarejeans_worn", "Hip Huggers"));
			charLeg.Items.Add(new RBString("frillyskirt_solid", "Frillseeker Skirt"));
			charLeg.Items.Add(new RBString("frillyundies_cotton", "Booty and the Beat"));
			charLeg.Items.Add(new RBString("gearheadleggings_leather", "Roadstar Leggings"));
			charLeg.Items.Add(new RBString("giantkillerleggings_metal", "Giant Killer Leggings"));
			charLeg.Items.Add(new RBString("gothmicrosand_stripedtights", "Candy Stripe Legs Micro"));
			charLeg.Items.Add(new RBString("gothmicrotight_rippedtights", "Rebel Rebel"));
			charLeg.Items.Add(new RBString("gothminiflare_barelegs", "Vinyl Maid Bare"));
			charLeg.Items.Add(new RBString("gothminiflare_rubber", "Vinyl Maid"));
			charLeg.Items.Add(new RBString("highwaterjeans_ripped", "Maine High Water Jeans"));
			charLeg.Items.Add(new RBString("hotpantsplain_barelegs", "Not Pants"));
			charLeg.Items.Add(new RBString("hotpantsplain_fishnetgothy", "Catch o' the Day"));
			charLeg.Items.Add(new RBString("hotpants_camo", "Camo Micro Shorts"));
			charLeg.Items.Add(new RBString("ironmaidenleggings_metal", "Titanium Maiden Leggings"));
			charLeg.Items.Add(new RBString("jumpsuitpants_nylon", "Might As Well Jumpsuit Slacks"));
			charLeg.Items.Add(new RBString("jumpsuitshorts_nylon", "Might As Well Jumpsuit Shorts"));
			charLeg.Items.Add(new RBString("kneeshorts_pinstripe", "Queen Britches"));
			charLeg.Items.Add(new RBString("lacedpants_leathers", "Pants Macabre"));
			charLeg.Items.Add(new RBString("laceskirt_krenelin", "Crinoline & Tears"));
			charLeg.Items.Add(new RBString("layeredlacedpants_leather", "Your Mama Don't Pants"));
			charLeg.Items.Add(new RBString("layeredpants_shredded", "Slashers"));
			charLeg.Items.Add(new RBString("leatherpants_solid", "Black Leathers"));
			charLeg.Items.Add(new RBString("leggings_shiny", "Heartbreaker Leggings"));
			charLeg.Items.Add(new RBString("linenskirt_twotone", "Miss Lizzy"));
			charLeg.Items.Add(new RBString("loincloth_metal", "Gird Up Your Loincloth"));
			charLeg.Items.Add(new RBString("loudleggings_acidwash", "Acid Flashback Leggings"));
			charLeg.Items.Add(new RBString("loudleggings_alphabet", "Alphabet City Leggings"));
			charLeg.Items.Add(new RBString("loudleggings_leopard", "Trashy Treasure Leggings"));
			charLeg.Items.Add(new RBString("loudleggings_solid", "Loud Leggings"));
			charLeg.Items.Add(new RBString("matadorpants_bedazzled", "Matadorable Leggings"));
			charLeg.Items.Add(new RBString("metalbottom_gold", "Metropolicious Bottoms"));
			charLeg.Items.Add(new RBString("metallicpants_metallic", "Lamr Leggings"));
			charLeg.Items.Add(new RBString("microskirt_bondage", "Safety Skirt"));
			charLeg.Items.Add(new RBString("microskirt_gothy", "Nevermore Micro"));
			charLeg.Items.Add(new RBString("miniskirtandleggings_worn", "Stevie Knickers"));
			charLeg.Items.Add(new RBString("miniskirt_leathergothy", "Haunt Me, Haunt Me Mini"));
			charLeg.Items.Add(new RBString("miniskirt_leatherpunky", "Bling Leather Mini"));
			charLeg.Items.Add(new RBString("miniskirt_leopard", "Bling Kitty"));
			charLeg.Items.Add(new RBString("mummyleggings_cotton", "Mummy Dearest"));
			charLeg.Items.Add(new RBString("nativepants_cotton", "Cataclysm Pants"));
			charLeg.Items.Add(new RBString("nauticaltrousers_canvas", "Sail Away Pants"));
			charLeg.Items.Add(new RBString("oilspillleggings_latex", "Citlalicue's Legs"));
			charLeg.Items.Add(new RBString("oringpants_snakeskin", "Snake-Os"));
			charLeg.Items.Add(new RBString("overskirt_leather", "Bandito Minito"));
			charLeg.Items.Add(new RBString("panelpants_plaidpunk", "Body Panels"));
			charLeg.Items.Add(new RBString("pantsspandex_spandex", "Packing Heat"));
			charLeg.Items.Add(new RBString("pedalpusherpants_army", "Patchy Pedal Pushers"));
			charLeg.Items.Add(new RBString("pleatedskirt_vinyl", "Nonsense and Sensibility"));
			charLeg.Items.Add(new RBString("puffyskirt_barelegs", "Puff Skirt"));
			charLeg.Items.Add(new RBString("puffyskirt_skull", "Skully Puff Skirt"));
			charLeg.Items.Add(new RBString("rolledpants_gothy", "Pixie Pants"));
			charLeg.Items.Add(new RBString("rolledpants_plaidkneepatch", "Annabelle Pants"));
			charLeg.Items.Add(new RBString("romperbottom_solid", "Romp Le Monde Bottoms"));
			charLeg.Items.Add(new RBString("rubberdress_pvc", "Rubber & Glue Skirt"));
			charLeg.Items.Add(new RBString("shortdress_satin", "Satin Island Fairy"));
			charLeg.Items.Add(new RBString("shortskirt_plaidbarelegs", "Plaid Day"));
			charLeg.Items.Add(new RBString("shortskirt_plaidpunky", "Tartan Skirt"));
			charLeg.Items.Add(new RBString("shortskirt_skullbarelegs", "Bare Your Skull"));
			charLeg.Items.Add(new RBString("shortskirt_skullgothy", "Skull & Goth Bones"));
			charLeg.Items.Add(new RBString("skinnyjeans_denim", "Skinny Jeans"));
			charLeg.Items.Add(new RBString("skinnypants_patchworkpunk", "Patchy Skinnys"));
			charLeg.Items.Add(new RBString("skirtpants_plaid", "Mini Grunge"));
			charLeg.Items.Add(new RBString("spikedleggings_leather", "Spiketress Leggings"));
			charLeg.Items.Add(new RBString("strapbodysuitbottom_leather", "Rapt and Strapped Bottoms"));
			charLeg.Items.Add(new RBString("stretchpants_leopard", "Stretchy Leopards"));
			charLeg.Items.Add(new RBString("studdedpants_leather", "Studcore Leather Pants"));
			charLeg.Items.Add(new RBString("suitbottom_solid", "Suit to Thrill Slacks"));
			charLeg.Items.Add(new RBString("tightcottonpants_pattern", "Sweet & Tartans"));
			charLeg.Items.Add(new RBString("tightjeans_faded", "Tragically Hipsters"));
			charLeg.Items.Add(new RBString("tightpants_checkered", "Checkers or Chess"));
			charLeg.Items.Add(new RBString("trackshorts_stripe", "Super Supergirl Shorts"));
			charLeg.Items.Add(new RBString("utilitygarter_fishnet", "Utilitease"));
			charLeg.Items.Add(new RBString("wornpleatedskirt_flower", "Wall Flowered"));
			charLeg.Items.Add(new RBString("wornpleatedskirt_plain", "Your Pleatin' Heart"));
			charLeg.Items.Add(new RBString("wornpleatedskirt_poodle", "Poodle Skirt"));
			charLeg.Items.Add(new RBString("wreckedjeans_worn", "Home Wreckers"));
		}
		private void AddMaleLeg()
		{
			charLeg.Items.Add(new RBString("", "Null (Invisible)"));
			charLeg.Items.Add(new RBString("male_legs_naked", "Nude"));
			charLeg.Items.Add(new RBString("basictightpants_jeans", "Texas Slims"));
			charLeg.Items.Add(new RBString("beltedspandex_lycra", "Leggings of the Beast"));
			charLeg.Items.Add(new RBString("bigzipperpants_denim", "Kohl Black Denim"));
			charLeg.Items.Add(new RBString("bikershortsflannel_spandex", "Short Pants O' Mine"));
			charLeg.Items.Add(new RBString("boapants_boaprint", "Boa Print Pants"));
			charLeg.Items.Add(new RBString("bondagepants_zippered", "Zipped Uprising"));
			charLeg.Items.Add(new RBString("boneleggings_leather", "Rock Hunter Leggings"));
			charLeg.Items.Add(new RBString("bootcutpants_distressedjeans", "Perfect Distress"));
			charLeg.Items.Add(new RBString("breechesnbraces_nylon", "Star Knight Breeches"));
			charLeg.Items.Add(new RBString("bulkycamopants_camo", "Tough Guy Camos"));
			charLeg.Items.Add(new RBString("buttlesschaps_leather", "Rearview Chaps"));
			charLeg.Items.Add(new RBString("camopants_canvas", "Guerilla Pantsfare"));
			charLeg.Items.Add(new RBString("carhartts_heavy", "Carpenter Pants"));
			charLeg.Items.Add(new RBString("chainleggings_metal", "Pelzgamaschen"));
			charLeg.Items.Add(new RBString("chaps_leather", "Chapped Legs"));
			charLeg.Items.Add(new RBString("checkeredpants_giraffe", "Tallboy"));
			charLeg.Items.Add(new RBString("checkeredpants_risingsun", "Rising Sun Pants"));
			charLeg.Items.Add(new RBString("checkeredpants_scales", "Arpeggio Pants"));
			charLeg.Items.Add(new RBString("checkeredpants_spandex", "Crazy Diamond Pants"));
			charLeg.Items.Add(new RBString("checkeredpants_spiral", "Optical Delusion"));
			charLeg.Items.Add(new RBString("checkeredpants_zebra", "Savannaflage"));
			charLeg.Items.Add(new RBString("classicmodslacks_cotton", "Flashback Slacks"));
			charLeg.Items.Add(new RBString("codpiecepants_robotic", "RoboCrotch"));
			charLeg.Items.Add(new RBString("corduroypants_worn", "Worn Cords"));
			charLeg.Items.Add(new RBString("cottondresspants_pinstripe", "Dolltree Pinstripes"));
			charLeg.Items.Add(new RBString("croppedbuttflapstrapped_cotton", "Pop Gothic Pants"));
			charLeg.Items.Add(new RBString("cutoffsflannel_jeans", "Roadie Shorts"));
			charLeg.Items.Add(new RBString("cutoffshorts_denim", "Ductwerk"));
			charLeg.Items.Add(new RBString("cutoffs_jeans", "Classic Cut-offs"));
			charLeg.Items.Add(new RBString("darkriderpants_cotton", "Little Lord Gothleroy"));
			charLeg.Items.Add(new RBString("draculaflairs_silk", "Funereal Flares"));
			charLeg.Items.Add(new RBString("fittedpants2_snake", "Boa Constrictors"));
			charLeg.Items.Add(new RBString("fittedpants_gator", "Stone Cold Reptiles"));
			charLeg.Items.Add(new RBString("flaredistressedpants_bleachedjeans", "Flare de Lis"));
			charLeg.Items.Add(new RBString("flightsuitpants_canvas", "Danger Zone Pants"));
			charLeg.Items.Add(new RBString("football_future", "Futuristic Footballer"));
			charLeg.Items.Add(new RBString("furshorts_fur", "Furbidden Love Briefs"));
			charLeg.Items.Add(new RBString("garteroring_fishnet", "You've Garter Hide Your Love Away"));
			charLeg.Items.Add(new RBString("gladiatorleggings_leather", "Gladiatorial Combat Leggings"));
			charLeg.Items.Add(new RBString("glamitardpants_mirrorball", "Eternal Glamnation Pants"));
			charLeg.Items.Add(new RBString("glamitardpants_wizard", "Pants of the Magic-User"));
			charLeg.Items.Add(new RBString("graverpants_blackwhiteredtrims", "Graver Specials"));
			charLeg.Items.Add(new RBString("grungepants_jeans", "Patchwork Tilt"));
			charLeg.Items.Add(new RBString("harajukubelt_camo", "Ura-Hara Camos"));
			charLeg.Items.Add(new RBString("harajukucutoffs_denim", "Peter Pants"));
			charLeg.Items.Add(new RBString("hipclipspants_leather", "Leather and Mace"));
			charLeg.Items.Add(new RBString("jams_floral", "Floral Boardies"));
			charLeg.Items.Add(new RBString("jams_hands", "Bitchin' Boardies"));
			charLeg.Items.Add(new RBString("jeansbelt_stripes", "Xavier Demon"));
			charLeg.Items.Add(new RBString("jeansbleached_denim", "Bleach Party"));
			charLeg.Items.Add(new RBString("jeansripped_denim", "Wanted: Shred or Alive"));
			charLeg.Items.Add(new RBString("jeansstudded_denim", "Studded Drainpipes"));
			charLeg.Items.Add(new RBString("kilt_plaid", "Kilty as Charged"));
			charLeg.Items.Add(new RBString("kissflares_sparkles", "Dressed to Kill Flares"));
			charLeg.Items.Add(new RBString("lacedside_pvc", "Laces & Vinyl"));
			charLeg.Items.Add(new RBString("lamepants_basicshiny", "Eltons"));
			charLeg.Items.Add(new RBString("leatherpants_basicleather", "Classic Leather Pants"));
			charLeg.Items.Add(new RBString("leatherslacks_roadwarrior", "Dystopia Pants"));
			charLeg.Items.Add(new RBString("leisurepants_verticalstripes", "Dolltree Leisure Pants"));
			charLeg.Items.Add(new RBString("longpantsstrapped_black", "Safety Pants"));
			charLeg.Items.Add(new RBString("loosegothtrousers_whitepiping", "Twice Shy Pants"));
			charLeg.Items.Add(new RBString("loosepants_stripe", "Ora Rocker Pants"));
			charLeg.Items.Add(new RBString("malebellbottoms_denim", "Loon Pants"));
			charLeg.Items.Add(new RBString("malebreeches_wool", "Proper Jodhpurs"));
			charLeg.Items.Add(new RBString("maleclamdiggers_slashed", "Truculent Pants"));
			charLeg.Items.Add(new RBString("malegothknickers_silk", "Kill'em Shakespeare Knickers"));
			charLeg.Items.Add(new RBString("malemetalpants_leather", "Beast of Burden Pants"));
			charLeg.Items.Add(new RBString("malerockpants_rhinestone", "Rhinestone Cowboy Pants"));
			charLeg.Items.Add(new RBString("pantsandlegwarmers_laced", "Legwarmers & Old Lace"));
			charLeg.Items.Add(new RBString("parachutepants_stitched", "Anarchy Flight Pants"));
			charLeg.Items.Add(new RBString("raggedyjeans_splattered", "Neoplastic Sarcastic Jeans"));
			charLeg.Items.Add(new RBString("relaxedpants_skatepunx", "Depleted Trousers"));
			charLeg.Items.Add(new RBString("rolledjeans_cowspot", "Hopeless Animal Jeans"));
			charLeg.Items.Add(new RBString("rolledpants_jeans", "St. Monday's"));
			charLeg.Items.Add(new RBString("rubberboys_redstrip", "Rubber Hose"));
			charLeg.Items.Add(new RBString("rusianpants_quilted", "Very Sharovary"));
			charLeg.Items.Add(new RBString("sashandpants_jeans", "Sash & Burn Jeans"));
			charLeg.Items.Add(new RBString("sexyshorts_cotton", "Long Story Shorts"));
			charLeg.Items.Add(new RBString("shreddedjeans_solid", "Shredded Shredders"));
			charLeg.Items.Add(new RBString("shreddedpants_marbleprint", "Lightning Strikes"));
			charLeg.Items.Add(new RBString("shredtights_nylon", "Ripped to Shreds"));
			charLeg.Items.Add(new RBString("sidedetailedpants_matador", "¡Toro! Trousers"));
			charLeg.Items.Add(new RBString("skullbaggyshorts_fishnets", "Skulking Shorts"));
			charLeg.Items.Add(new RBString("slicedpants_jeans", "Ready, Shreddy, Go!"));
			charLeg.Items.Add(new RBString("spikedarmorleggings_spandex", "Oni Kneestabbers"));
			charLeg.Items.Add(new RBString("sportbikeleggings_leather", "Hanzo Leathers"));
			charLeg.Items.Add(new RBString("strappants_leather", "Nocturnals"));
			charLeg.Items.Add(new RBString("strappedpants_parachute", "Life Savers"));
			charLeg.Items.Add(new RBString("streamerpants_spandex", "Iron Rainbow Leggings"));
			charLeg.Items.Add(new RBString("strippedpants_ornate", "Ornate or Not Pants"));
			charLeg.Items.Add(new RBString("suitpants_cotton", "Working Man Slacks"));
			charLeg.Items.Add(new RBString("summonerleggings_burlap", "Summoner's Leggings"));
			charLeg.Items.Add(new RBString("suspendedpants_plaid", "Molotov Long Shorts"));
			charLeg.Items.Add(new RBString("suspenderpants_frayed", "Graveyard Issue"));
			charLeg.Items.Add(new RBString("tightdistressedpants_jeans", "Seattle Specials"));
			charLeg.Items.Add(new RBString("tighterpants_plaidpunx", "Wreckless Pants"));
			charLeg.Items.Add(new RBString("tightjeans_denim", "Hamhuggers"));
			charLeg.Items.Add(new RBString("tightpants_sequins", "Sequintial"));
			charLeg.Items.Add(new RBString("tightpants_skeletonprint", "Skelly Pants"));
			charLeg.Items.Add(new RBString("tuxpants_sloppy", "Sloppy Slacks"));
			charLeg.Items.Add(new RBString("ultimatepunklegs", "Punk and Disorderly Briefs"));
			charLeg.Items.Add(new RBString("undies_dirty", "Brief Examples"));
			charLeg.Items.Add(new RBString("whiteflaircumber_cotton", "Mighty Mercury's"));
			charLeg.Items.Add(new RBString("wizardpants_dragons", "Dungarees and Dragons"));
			charLeg.Items.Add(new RBString("workpants_grimey", "Working Overtime"));
			charLeg.Items.Add(new RBString("wornleatherpants_worn", "Mole Skin Leather Pants"));
			charLeg.Items.Add(new RBString("zipperpants_panels", "DIY Nightmare"));
		}
		private void AddFemTorso()
		{
			charTorso.Items.Add(new RBString("", "Null (Invisible)"));
			charTorso.Items.Add(new RBString("female_torso_naked", "Nude"));
			charTorso.Items.Add(new RBString("aodaishirt_smoke", "Ao Dai Top"));
			charTorso.Items.Add(new RBString("armorstuddedjacket_leather", "Articulated Leather Jacket"));
			charTorso.Items.Add(new RBString("aztecarmor_hide", "Toci's Chest"));
			charTorso.Items.Add(new RBString("baseballteetorn_stitched", "Open Heart Serger Tee"));
			charTorso.Items.Add(new RBString("baseballtee_10k", "10K Fun Run"));
			charTorso.Items.Add(new RBString("baseballtee_ernieball", "Ernie Ball Shirt"));
			charTorso.Items.Add(new RBString("baseballtee_fender", "Fender Shirt"));
			charTorso.Items.Add(new RBString("baseballtee_musclecar", "Muscle Car"));
			charTorso.Items.Add(new RBString("baseballtee_promark", "Promark Baseball Tee"));
			charTorso.Items.Add(new RBString("baseballtee_solid", "American Girl"));
			charTorso.Items.Add(new RBString("basictee_chopper", "Rossi's Motorcycles"));
			charTorso.Items.Add(new RBString("basictee_dwdrums", "DW Drums Tee"));
			charTorso.Items.Add(new RBString("basictee_eh", "Electro-Harmonix Tee"));
			charTorso.Items.Add(new RBString("basictee_emg", "EMG Tee"));
			charTorso.Items.Add(new RBString("basictee_ernieballcrest", "Ernie Ball Crest Tee"));
			charTorso.Items.Add(new RBString("basictee_gretschdrums", "Gretsch Drums Tee"));
			charTorso.Items.Add(new RBString("basictee_ludwigsticks", "Ludwig Tee"));
			charTorso.Items.Add(new RBString("basictee_pearlwings", "Pearl Tee"));
			charTorso.Items.Add(new RBString("basictee_phase", "Phase Tee"));
			charTorso.Items.Add(new RBString("basictee_proliferation", "Nuclear Proliferation Tee"));
			charTorso.Items.Add(new RBString("basictee_rbbass", "Bass Tee"));
			charTorso.Items.Add(new RBString("basictee_rbdrums", "Drums Tee"));
			charTorso.Items.Add(new RBString("basictee_rbfour", "All Four Tee"));
			charTorso.Items.Add(new RBString("basictee_rbguitar", "Guitar Tee"));
			charTorso.Items.Add(new RBString("basictee_rbkeys", "Keys Tee"));
			charTorso.Items.Add(new RBString("basictee_rbmic", "Vox Tee"));
			charTorso.Items.Add(new RBString("basictee_ringer", "Dead Ringer"));
			charTorso.Items.Add(new RBString("basictee_rockbandlogo", "The Rock Band Tee"));
			charTorso.Items.Add(new RBString("basictee_sennheiser", "Sennheiser Tee"));
			charTorso.Items.Add(new RBString("basictee_solid", "Thrashin' Tee"));
			charTorso.Items.Add(new RBString("basictee_tiedye", "Fit to be Tie-Dyed"));
			charTorso.Items.Add(new RBString("basictee_zvex", "Z.Vex Tee"));
			charTorso.Items.Add(new RBString("batvneck_cotton", "Mistress of the Night V-Neck"));
			charTorso.Items.Add(new RBString("bigstraps_rubber", "Back in Straps"));
			charTorso.Items.Add(new RBString("bikinichain_camo", "Panzer Girl"));
			charTorso.Items.Add(new RBString("bikinichain_chainmail", "Ch-ch-chain Is"));
			charTorso.Items.Add(new RBString("bikinichain_lace", "Lace Trace"));
			charTorso.Items.Add(new RBString("bikinichain_leopard", "You Jane"));
			charTorso.Items.Add(new RBString("bikinichain_metal", "Breastplate"));
			charTorso.Items.Add(new RBString("bikinichain_print", "Atom Splitter"));
			charTorso.Items.Add(new RBString("bikinichain_skull", "Skull Crossed"));
			charTorso.Items.Add(new RBString("bikinichain_solid", "Solid Bikini"));
			charTorso.Items.Add(new RBString("bikinichain_swirl", "Wave Rider"));
			charTorso.Items.Add(new RBString("bikinichain_ukflag", "Union Bikini"));
			charTorso.Items.Add(new RBString("blazer_newwave", "Trail Blazer"));
			charTorso.Items.Add(new RBString("boxytee_cateyes", "Cat Scan"));
			charTorso.Items.Add(new RBString("boxytee_cotton", "...What a Tee-shirt!"));
			charTorso.Items.Add(new RBString("boxytee_football", "Football Jersey"));
			charTorso.Items.Add(new RBString("boxytee_wingedserpent", "Winged Serpent"));
			charTorso.Items.Add(new RBString("buckletank_leather", "Buckled Under Pressure"));
			charTorso.Items.Add(new RBString("bulkyjacket_camo", "Grandpa's Camo Coat"));
			charTorso.Items.Add(new RBString("bulkyjacket_canvas", "Apathetic Aesthetic"));
			charTorso.Items.Add(new RBString("busselcoat_lace", "Anne of Seven Gables"));
			charTorso.Items.Add(new RBString("capedracula_silk", "Black Widow Cape"));
			charTorso.Items.Add(new RBString("capepointyfemale_silk", "Crimefighter Cape"));
			charTorso.Items.Add(new RBString("caperoundedfemale_silk", "Cape Odd"));
			charTorso.Items.Add(new RBString("carcass_leather", "Fox on the Run"));
			charTorso.Items.Add(new RBString("choppedtee_chopped", "Bad Reputation"));
			charTorso.Items.Add(new RBString("choppedtee_chopped_300lbs", "300 lbs. Club"));
			charTorso.Items.Add(new RBString("cinchshirt_solid", "'It's a Cinch' Top"));
			charTorso.Items.Add(new RBString("clearcoat_plastic", "Right as Raincoat"));
			charTorso.Items.Add(new RBString("coatdress_solid", "It's a Mod, Mod, Mod, Mod World"));
			charTorso.Items.Add(new RBString("corsetandsleeves_plastic", "Evil Corset"));
			charTorso.Items.Add(new RBString("corsetblouse_silk", "Take a Bow"));
			charTorso.Items.Add(new RBString("corsetcombo_cotton", "Angelic Corset"));
			charTorso.Items.Add(new RBString("corset_camo", "Battlefield Corset"));
			charTorso.Items.Add(new RBString("corset_metal", "Metacular Corset"));
			charTorso.Items.Add(new RBString("corset_vinyl", "Beatrix"));
			charTorso.Items.Add(new RBString("costumejacket_paisley", "God Save the Queen"));
			charTorso.Items.Add(new RBString("croppedjacket_bondage", "Not So Straitjacket"));
			charTorso.Items.Add(new RBString("dapperjacket_pinstripe", "LA Woman"));
			charTorso.Items.Add(new RBString("dapperjacketbandana_embroidered", "Simply Irresistible"));
			charTorso.Items.Add(new RBString("demonarmor_lace", "Demons & Lace Top"));
			charTorso.Items.Add(new RBString("denimjacket_clean", "Pristine Jean Jacket"));
			charTorso.Items.Add(new RBString("denimjacket_patched", "Dirty Denim Jacket"));
			charTorso.Items.Add(new RBString("dishevelledshirt_southern", "Hells Belle"));
			charTorso.Items.Add(new RBString("dogcollar_leather", "Hot Under the Collar"));
			charTorso.Items.Add(new RBString("dragonslayerarmor_scales", "Wyvern Slayer Armor"));
			charTorso.Items.Add(new RBString("eightiesjacket_leather", "'80s Jacket"));
			charTorso.Items.Add(new RBString("feathercorsettop_top", "Par for the Corset Top"));
			charTorso.Items.Add(new RBString("femalebra_cotton", "Shirtless"));
			charTorso.Items.Add(new RBString("femalespikedarmor_leather", "Spiketress Armor"));
			charTorso.Items.Add(new RBString("female_premium_doors_band", "The Doors Band Tee"));
			charTorso.Items.Add(new RBString("female_premium_doors_jim", "The Doors Morrison Tee"));
			charTorso.Items.Add(new RBString("female_premium_doors_logo", "The Doors Logo Tee"));
			charTorso.Items.Add(new RBString("female_premium_hendrix_electricladyland", "Jimi Hendrix Electric Ladyland Tee"));
			charTorso.Items.Add(new RBString("female_premium_hendrix_experienced", "Jimi Hendrix Are You Experienced Tee"));
			charTorso.Items.Add(new RBString("female_premium_hendrix_guitarface", "Jimi Hendrix Guitar Tee"));
			charTorso.Items.Add(new RBString("female_premium_hendrix_gypsys", "Jimi Hendrix Band of Gypsys Tee"));
			charTorso.Items.Add(new RBString("female_premium_hendrix_rockpose", "Jimi Hendrix Rock Pose Tee"));
			charTorso.Items.Add(new RBString("female_premium_hendrix_valleysofneptune", "Jimi Hendrix Valleys of Neptune Tee"));
			charTorso.Items.Add(new RBString("female_premium_hendrix_woodstock", "Jimi Hendrix Woodstock Tee"));
			charTorso.Items.Add(new RBString("female_premium_marley_exodus", "Bob Marley Exodus Tee"));
			charTorso.Items.Add(new RBString("female_premium_marley_flagface", "Bob Marley Legacy Tee"));
			charTorso.Items.Add(new RBString("female_premium_marley_lion", "Bob Marley Lion Tee"));
			charTorso.Items.Add(new RBString("female_premium_marley_soulrebel", "Bob Marley Soul Rebel Tee"));
			charTorso.Items.Add(new RBString("female_premium_marley_trenchtown", "Bob Marley Trenchtown Tee"));
			charTorso.Items.Add(new RBString("female_premium_marley_vintage", "Bob Marley Vintage Tee"));
			charTorso.Items.Add(new RBString("female_premium_metallica_logo", "Metallica Classic Logo Tee"));
			charTorso.Items.Add(new RBString("female_premium_spinaltap_flamelogo", "Spinal Tap Flame Logo Tee"));
			charTorso.Items.Add(new RBString("female_premium_spinaltap_intravenus", "Spinal Tap Intravenus de Milo Tee"));
			charTorso.Items.Add(new RBString("female_premium_spinaltap_metallogo", "Spinal Tap Classic Logo Tee"));
			charTorso.Items.Add(new RBString("female_premium_spinaltap_sharksandwich", "Spinal Tap Shark Sandwich Tee"));
			charTorso.Items.Add(new RBString("female_premium_spinaltap_skeleton", "Spinal Tap Skeleton Tee"));
			charTorso.Items.Add(new RBString("female_premium_spinaltap_stonehenge", "Spinal Tap Stonehenge Tee"));
			charTorso.Items.Add(new RBString("female_premium_who_fist", "The Who Triumph Tee"));
			charTorso.Items.Add(new RBString("female_premium_who_flag", "The Who Union Jack Tee"));
			charTorso.Items.Add(new RBString("female_premium_who_flagjump", "The Who Rock Jump Tee"));
			charTorso.Items.Add(new RBString("female_premium_who_maximumrandb", "The Who Maximum R&B Tee"));
			charTorso.Items.Add(new RBString("female_premium_who_splits", "The Who Split Jump Tee"));
			charTorso.Items.Add(new RBString("female_premium_who_target", "The Who Target Logo Tee"));
			charTorso.Items.Add(new RBString("femgothcorset_satin", "Mourning Glory Corset"));
			charTorso.Items.Add(new RBString("femmetaltop_snake", "Venom Vixen Top"));
			charTorso.Items.Add(new RBString("fempunktop_tape", "Punks Not Threads Top"));
			charTorso.Items.Add(new RBString("femrockvest_leather", "Worn in the USA Vest"));
			charTorso.Items.Add(new RBString("flightjacket_solid", "MA-1 Classic Flight Jacket"));
			charTorso.Items.Add(new RBString("frankensteintee_cannibal", "Catch as Catch Cannibal"));
			charTorso.Items.Add(new RBString("frankensteintee_commie", "Frankenstein's T-Shirt"));
			charTorso.Items.Add(new RBString("furbikini_cheetah", "Sweet Cheetah"));
			charTorso.Items.Add(new RBString("furbikini_plain", "Barbarian Princess"));
			charTorso.Items.Add(new RBString("furbikini_tiger", "Easy Tiger"));
			charTorso.Items.Add(new RBString("furcoat_faux", "Friend or Faux"));
			charTorso.Items.Add(new RBString("furhoodie_leather", "Fur Elise"));
			charTorso.Items.Add(new RBString("furtrimjacket_nylon", "Coat of Our Discontent"));
			charTorso.Items.Add(new RBString("gearheadarmor_leather", "Roadstar Armor"));
			charTorso.Items.Add(new RBString("giantkillerarmor_metal", "Giant Killer Armor"));
			charTorso.Items.Add(new RBString("halfshirt_reformschool", "Rockington Reform Uniform"));
			charTorso.Items.Add(new RBString("halfsleeveschooltop_cotton", "Weepy Hollows Top"));
			charTorso.Items.Add(new RBString("halter_cotton", "Club Girl Halter"));
			charTorso.Items.Add(new RBString("halter_disco", "C'est Chic Halter"));
			charTorso.Items.Add(new RBString("highwaist_whool", "High-waisted Dress"));
			charTorso.Items.Add(new RBString("hippyfringe_cotton", "Flower Child"));
			charTorso.Items.Add(new RBString("hoodedsweatshirt_plain", "Good Hoodie"));
			charTorso.Items.Add(new RBString("hoodedsweatshirt_skullflower", "Skull Flowers"));
			charTorso.Items.Add(new RBString("hoodie_patchy", "The Right Stripes"));
			charTorso.Items.Add(new RBString("ironmaidenarmor_metal", "Titanium Maiden Armor"));
			charTorso.Items.Add(new RBString("jacketandbra_solid", "Like a Virgin"));
			charTorso.Items.Add(new RBString("jackettubetop_cotton", "Pro and Con Blazer"));
			charTorso.Items.Add(new RBString("jumpsuittop_nylon", "Might As Well Jumpsuit Top"));
			charTorso.Items.Add(new RBString("kimono_pattern1", "Lovely Assassin"));
			charTorso.Items.Add(new RBString("ladykeffiyeh_cotton", "Scarfed for Life"));
			charTorso.Items.Add(new RBString("layeredtanktop_threelayers", "Starlight"));
			charTorso.Items.Add(new RBString("leatherjacket_clean", "Defiant Leathers"));
			charTorso.Items.Add(new RBString("leatherplaid_leather", "Butch Cassidy"));
			charTorso.Items.Add(new RBString("lolitatop_cotton", "Annabelle Lee"));
			charTorso.Items.Add(new RBString("longsleevedress_floral", "Hot Couture Dress"));
			charTorso.Items.Add(new RBString("longsleevedress_victorian", "Faithful Shift"));
			charTorso.Items.Add(new RBString("maidenstraps_studded", "Metal Goddess Bodice"));
			charTorso.Items.Add(new RBString("matadorjacket_bedazzled", "Matadorable Jacket"));
			charTorso.Items.Add(new RBString("metaltop_gold", "Metropolicious Top"));
			charTorso.Items.Add(new RBString("metaltri_leather", "Love Triangle Jacket"));
			charTorso.Items.Add(new RBString("moddress_diamonds", "Geometric Figures"));
			charTorso.Items.Add(new RBString("moddress_dottree", "Simply Mod"));
			charTorso.Items.Add(new RBString("moddress_doubledots", "Maximum Mini"));
			charTorso.Items.Add(new RBString("moddress_plain", "Art School Dress"));
			charTorso.Items.Add(new RBString("motojacket_solid", "Lost My License"));
			charTorso.Items.Add(new RBString("mummyjacket_cotton", "Mummy's Wrap"));
			charTorso.Items.Add(new RBString("netdress_net", "Netting But a Good Time"));
			charTorso.Items.Add(new RBString("oilspillarmor_latex", "Citlalicue's Chest"));
			charTorso.Items.Add(new RBString("openshirtbandana_print", "Chains of Love"));
			charTorso.Items.Add(new RBString("openshirt_solid", "Lucky Star"));
			charTorso.Items.Add(new RBString("parachute_nylon", "Chute the Moon"));
			charTorso.Items.Add(new RBString("parkajacket_parka", "Nikola Parka"));
			charTorso.Items.Add(new RBString("puffedsleeves_leather", "Pretty in Black"));
			charTorso.Items.Add(new RBString("puffydress_solid", "Atomic Prom"));
			charTorso.Items.Add(new RBString("raincoat_rubber", "Restrain-coat"));
			charTorso.Items.Add(new RBString("ramskullarmor_metal", "Ram it Home Skull Top"));
			charTorso.Items.Add(new RBString("rompertop_solid", "Romp Le Monde Top"));
			charTorso.Items.Add(new RBString("roundtopdress_polkadot", "Polka Dot Pin-up"));
			charTorso.Items.Add(new RBString("ruffledsuitjacket_satin", "The Jacket Formerly Known as Ruffled"));
			charTorso.Items.Add(new RBString("satinadmiral_ornate", "Dance Commander"));
			charTorso.Items.Add(new RBString("scarftanktop_threelayers", "Melange A Trois"));
			charTorso.Items.Add(new RBString("schoolgirlshirt_dishevelled", "Rockington Prep Uniform"));
			charTorso.Items.Add(new RBString("shirtcorset_solid", "Suspension of Disbelief"));
			charTorso.Items.Add(new RBString("silkfrills_silk", "Beverly Frills"));
			charTorso.Items.Add(new RBString("sleevelessbuttonflap_leather", "Lonely Hearts Club"));
			charTorso.Items.Add(new RBString("sleevlesshoodie_solid", "Hood Intentions"));
			charTorso.Items.Add(new RBString("spikedleatherjacket_spiked", "Coatatonic"));
			charTorso.Items.Add(new RBString("spikepads_distressed", "Spaulders of Metal"));
			charTorso.Items.Add(new RBString("strapbodysuit_leather", "Rapt and Strapped Top"));
			charTorso.Items.Add(new RBString("stripedshirt_cotton", "Earn Your Stripes Top"));
			charTorso.Items.Add(new RBString("studdedhoody_cotton", "Mesh Missiles Hoodie"));
			charTorso.Items.Add(new RBString("studdedjacketandtee_wornleather", "Stud Minder"));
			charTorso.Items.Add(new RBString("suittop_solid", "Suit to Thrill Blazer"));
			charTorso.Items.Add(new RBString("suspenders_solid", "Mama Mia"));
			charTorso.Items.Add(new RBString("tanktop_prismaticcheetah", "Sleeveless in Seattle"));
			charTorso.Items.Add(new RBString("tanktop_solid", "Top Tank"));
			charTorso.Items.Add(new RBString("thermalshirt_threadbare", "GoTag Thermal"));
			charTorso.Items.Add(new RBString("thermalundertee_moped", "Moped Devotee"));
			charTorso.Items.Add(new RBString("thermalundertee_panther", "Cyber Panther"));
			charTorso.Items.Add(new RBString("thermalundertee_thermal", "Therma-stat"));
			charTorso.Items.Add(new RBString("thicksweater_damaged", "Undone Sweater"));
			charTorso.Items.Add(new RBString("thintank_printed", "Magic Tanktop Ride"));
			charTorso.Items.Add(new RBString("thintank_solid", "Camden Calling Cami"));
			charTorso.Items.Add(new RBString("tiedshirt_plaid", "Plaid Reputation"));
			charTorso.Items.Add(new RBString("tiedshirt_plain", "Nights in Tight Satin"));
			charTorso.Items.Add(new RBString("tiedtube_cotton", "Bandeau Ballet"));
			charTorso.Items.Add(new RBString("tirearmor_rubber", "Tire Armor"));
			charTorso.Items.Add(new RBString("trackjacket_solid", "On the Right Track Jacket"));
			charTorso.Items.Add(new RBString("triceratopsvest_worndenim", "TriceraTops Vest"));
			charTorso.Items.Add(new RBString("tubetop_solid", "Venus Fly Trap"));
			charTorso.Items.Add(new RBString("tunicoverlap_paisley", "Paisley Amazing Tunic"));
			charTorso.Items.Add(new RBString("tunicoverlap_solid", "12th Night Tunic"));
			charTorso.Items.Add(new RBString("tunicstitch_indian", "Kashmir Tunic"));
			charTorso.Items.Add(new RBString("tunicstitch_solid", "Stitchin' Time"));
			charTorso.Items.Add(new RBString("turtledress_paisley", "Hell to Paisley Dress"));
			charTorso.Items.Add(new RBString("turtledress_plain", "Plain and Simple Dress"));
			charTorso.Items.Add(new RBString("turtledress_squares", "Fair and Square Dress"));
			charTorso.Items.Add(new RBString("turtleneckmodern_cotton", "Hickey Hider"));
			charTorso.Items.Add(new RBString("turtleneck_occult", "Hell and Ready"));
			charTorso.Items.Add(new RBString("tuxjacket_silk", "Melancholy Prom"));
			charTorso.Items.Add(new RBString("vestandlongsleeve_denim", "Perforations Vest"));
			charTorso.Items.Add(new RBString("vestandtank_cotton", "How the Vest was Worn"));
			charTorso.Items.Add(new RBString("vestgasmask_studded", "Apocalypse Girl"));
			charTorso.Items.Add(new RBString("vest_studded", "Spike"));
			charTorso.Items.Add(new RBString("vneck_cotton", "Good Gal V-neck"));
			charTorso.Items.Add(new RBString("westernfringe_applique", "Achy Breaky Shirt"));
			charTorso.Items.Add(new RBString("zebrachoptop_silk", "A-to-Zebra Silk Top"));
		}
		private void AddMaleTorso()
		{
			charTorso.Items.Add(new RBString("", "Null (Invisible)"));
			charTorso.Items.Add(new RBString("male_torso_naked", "Shirtless"));
			charTorso.Items.Add(new RBString("admiraljacket_ornate", "The Duke of Jackets"));
			charTorso.Items.Add(new RBString("alexjacket_leather", "The CEO My Goodness Jacket"));
			charTorso.Items.Add(new RBString("armorshirt_japaneseprint", "Okiro Armor"));
			charTorso.Items.Add(new RBString("armyjacket_dirty", "Underclass Reject"));
			charTorso.Items.Add(new RBString("barbarianbelt_iron", "Belt of the Usurper"));
			charTorso.Items.Add(new RBString("bedazzledversaillesjacket_sparkly", "Regal Vestiges"));
			charTorso.Items.Add(new RBString("beltedtee_fishnetsleeves", "Beau Filet"));
			charTorso.Items.Add(new RBString("blankhoodie_ludwigsticks", "Ludwig Hoodie"));
			charTorso.Items.Add(new RBString("blankhoodie_solid", "Good Hoodie"));
			charTorso.Items.Add(new RBString("blazershirtless_blazer", "Chest for Success"));
			charTorso.Items.Add(new RBString("blousey_cotton", "Modest Blouse"));
			charTorso.Items.Add(new RBString("bondagerings_leather", "Male Bondage"));
			charTorso.Items.Add(new RBString("boneharness_leather", "Heart of Bone"));
			charTorso.Items.Add(new RBString("bulletproof_kevlar", "Going Ballistic Vest"));
			charTorso.Items.Add(new RBString("buttonedshirtchestpockets_denim", "Chambre Shirt"));
			charTorso.Items.Add(new RBString("buttonedshirtpatchy_veteran", "AF Shirt"));
			charTorso.Items.Add(new RBString("buttonedshirt_hippie", "The Floridian"));
			charTorso.Items.Add(new RBString("buttonedshirt_southern", "Alabama Slamma Button-down"));
			charTorso.Items.Add(new RBString("buzzsawpads_wrecked", "Lawless Pauldrons"));
			charTorso.Items.Add(new RBString("camotanktop_cotton", "Camoflange"));
			charTorso.Items.Add(new RBString("camotanktop_ernieball", "Ernie Ball Tank"));
			charTorso.Items.Add(new RBString("camotanktop_zildjian", "Zildjian Tank"));
			charTorso.Items.Add(new RBString("capelettespiderpins_dark", "Spider-Silk Capelet"));
			charTorso.Items.Add(new RBString("capepointymale_silk", "Crimefighter Cape"));
			charTorso.Items.Add(new RBString("caperoundedmale_silk", "Cape Odd"));
			charTorso.Items.Add(new RBString("casualmodjacket_tweed", "Modern Lover"));
			charTorso.Items.Add(new RBString("chainshirt_metal", "Pelzhemd"));
			charTorso.Items.Add(new RBString("corset_leather", "Arockalypse Now"));
			charTorso.Items.Add(new RBString("croppedtailscoat_furredcollar", "Libertinette"));
			charTorso.Items.Add(new RBString("customleatherjacket_old", "Thunder Road"));
			charTorso.Items.Add(new RBString("cutoffjeanjacket_denim", "DIY Vest"));
			charTorso.Items.Add(new RBString("dapperscarf_leopard", "Glutton of Privilege"));
			charTorso.Items.Add(new RBString("dapperscarf_zebra", "Mr. Zebra"));
			charTorso.Items.Add(new RBString("darkdandicoat_black", "Majorca Coat"));
			charTorso.Items.Add(new RBString("deepcutsleeves_cotton", "Bicep Brandisher"));
			charTorso.Items.Add(new RBString("deepcutsleeves_sync", "Ripped Tee"));
			charTorso.Items.Add(new RBString("doublehoodie_punky", "Rat Bastard Hoodie"));
			charTorso.Items.Add(new RBString("downvest_stripe", "Triple Fat Moose Vest"));
			charTorso.Items.Add(new RBString("draculacape_silk", "Dr. Acula Cape"));
			charTorso.Items.Add(new RBString("drugrug_poncho", "Baja Poncho"));
			charTorso.Items.Add(new RBString("escapeartist_canvas", "Escape Artist"));
			charTorso.Items.Add(new RBString("flannelcoat_checked", "Flannel Ledbetter"));
			charTorso.Items.Add(new RBString("formalvest_cotton", "Vested Interest"));
			charTorso.Items.Add(new RBString("frillyshirt_solid", "Highland Ruffles"));
			charTorso.Items.Add(new RBString("fringeshirt_denim", "Fringe Benefit"));
			charTorso.Items.Add(new RBString("fringeshirt_rhinestone", "Viva Las Vegas"));
			charTorso.Items.Add(new RBString("furvest_fur", "Faux Certain Vest"));
			charTorso.Items.Add(new RBString("gatorfutureshirt_leather", "Metal Demon Shirt"));
			charTorso.Items.Add(new RBString("gi_silk", "Continental Kimono"));
			charTorso.Items.Add(new RBString("gladiatorvest_leather", "Arena Master"));
			charTorso.Items.Add(new RBString("glamitardtop_mirrorball", "Eternal Glamnation Top"));
			charTorso.Items.Add(new RBString("glamitardtop_wizard", "Top of the Magic-User"));
			charTorso.Items.Add(new RBString("greaserjacket_leather", "Rebel Without Applause"));
			charTorso.Items.Add(new RBString("halfshirtbracers_cotton", "Metal to the Bone"));
			charTorso.Items.Add(new RBString("halfshirt_halfshirt", "Hafta Halfshirt"));
			charTorso.Items.Add(new RBString("harnesedtop_pvc", "Truss-Tee"));
			charTorso.Items.Add(new RBString("harness_leather", "Halford Harness"));
			charTorso.Items.Add(new RBString("highneck_mesh", "Pectacular Top"));
			charTorso.Items.Add(new RBString("hoodieandjacket_leather", "Leather Jacket Weather"));
			charTorso.Items.Add(new RBString("hoodiejacket_studs", "Strummer Studs"));
			charTorso.Items.Add(new RBString("hoodiesleeveless_solid", "The Amateur Deejay"));
			charTorso.Items.Add(new RBString("indianchest_bone", "Dream Catcher"));
			charTorso.Items.Add(new RBString("jacketvestnoshirt_cotton", "Barrio Lothario"));
			charTorso.Items.Add(new RBString("jeanjacket_denim", "Judas Jean"));
			charTorso.Items.Add(new RBString("junkyard_rusty", "Junkyard Gladiator"));
			charTorso.Items.Add(new RBString("kissarmor_sparkles", "Dressed to Kill Armor"));
			charTorso.Items.Add(new RBString("latexwarrior_pvc", "God of Latex Attire"));
			charTorso.Items.Add(new RBString("leathershoulderstraps_twotone", "Princeps"));
			charTorso.Items.Add(new RBString("leathervest_studded", "Cougar Army"));
			charTorso.Items.Add(new RBString("lightsanimated_plastic", "The Independent Party"));
			charTorso.Items.Add(new RBString("longjacket_faceshirt", "Doctor What"));
			charTorso.Items.Add(new RBString("longsleeveshirt_dwdrums", "DW Shirt"));
			charTorso.Items.Add(new RBString("longsleeveshirt_gretschdrums", "Gretsch Drums Shirt"));
			charTorso.Items.Add(new RBString("longsleeveshirt_plain", "Classic Long-sleeve Tee"));
			charTorso.Items.Add(new RBString("longsleeveshirt_promark", "Promark Shirt"));
			charTorso.Items.Add(new RBString("longsleeveshirt_skeletonprint", "Skelly Shirt"));
			charTorso.Items.Add(new RBString("loosedapperjacket_tweed", "Professor"));
			charTorso.Items.Add(new RBString("loosedapperjacket_vintage", "Vintage Blazer and Tie"));
			charTorso.Items.Add(new RBString("macho_leather", "Macho Man"));
			charTorso.Items.Add(new RBString("malediscoshirt_floral", "Floral Values"));
			charTorso.Items.Add(new RBString("malediscoshirt_polyester", "Polyester Inferno"));
			charTorso.Items.Add(new RBString("malegothblouse_silk", "Kill'em Shakespeare Blouse"));
			charTorso.Items.Add(new RBString("malemetaltunic_hide", "Beast of Burden Tunic"));
			charTorso.Items.Add(new RBString("malerockjacket_rhinestone", "Rhinestone Cowboy Jacket"));
			charTorso.Items.Add(new RBString("male_premium_doors_band", "The Doors Band Tee"));
			charTorso.Items.Add(new RBString("male_premium_doors_jim", "The Doors Morrison Tee"));
			charTorso.Items.Add(new RBString("male_premium_doors_logo", "The Doors Logo Tee"));
			charTorso.Items.Add(new RBString("male_premium_hendrix_electricladyland", "Jimi Hendrix Electric Ladyland Tee"));
			charTorso.Items.Add(new RBString("male_premium_hendrix_experienced", "Jimi Hendrix Are You Experienced Tee"));
			charTorso.Items.Add(new RBString("male_premium_hendrix_guitarface", "Jimi Hendrix Guitar Tee"));
			charTorso.Items.Add(new RBString("male_premium_hendrix_gypsys", "Jimi Hendrix Band of Gypsys Tee"));
			charTorso.Items.Add(new RBString("male_premium_hendrix_rockpose", "Jimi Hendrix Rock Pose Tee"));
			charTorso.Items.Add(new RBString("male_premium_hendrix_valleysofneptune", "Jimi Hendrix Valleys of Neptune Tee"));
			charTorso.Items.Add(new RBString("male_premium_hendrix_woodstock", "Jimi Hendrix Woodstock Tee"));
			charTorso.Items.Add(new RBString("male_premium_marley_exodus", "Bob Marley Exodus Tee"));
			charTorso.Items.Add(new RBString("male_premium_marley_flagface", "Bob Marley Legacy Tee"));
			charTorso.Items.Add(new RBString("male_premium_marley_lion", "Bob Marley Lion Tee"));
			charTorso.Items.Add(new RBString("male_premium_marley_soulrebel", "Bob Marley Soul Rebel Tee"));
			charTorso.Items.Add(new RBString("male_premium_marley_trenchtown", "Bob Marley Trenchtown Tee"));
			charTorso.Items.Add(new RBString("male_premium_marley_vintage", "Bob Marley Vintage Tee"));
			charTorso.Items.Add(new RBString("male_premium_metallica_logo", "Metallica Classic Logo Tee"));
			charTorso.Items.Add(new RBString("male_premium_spinaltap_flamelogo", "Spinal Tap Flame Logo Tee"));
			charTorso.Items.Add(new RBString("male_premium_spinaltap_intravenus", "Spinal Tap Intravenus de Milo Tee"));
			charTorso.Items.Add(new RBString("male_premium_spinaltap_metallogo", "Spinal Tap Classic Logo Tee"));
			charTorso.Items.Add(new RBString("male_premium_spinaltap_sharksandwich", "Spinal Tap Shark Sandwich Tee"));
			charTorso.Items.Add(new RBString("male_premium_spinaltap_skeleton", "Spinal Tap Skeleton Tee"));
			charTorso.Items.Add(new RBString("male_premium_spinaltap_stonehenge", "Spinal Tap Stonehenge Tee"));
			charTorso.Items.Add(new RBString("male_premium_who_fist", "The Who Triumph Tee"));
			charTorso.Items.Add(new RBString("male_premium_who_flag", "The Who Union Jack Tee"));
			charTorso.Items.Add(new RBString("male_premium_who_flagjump", "The Who Rock Jump Tee"));
			charTorso.Items.Add(new RBString("male_premium_who_maximumrandb", "The Who Maximum R&B Tee"));
			charTorso.Items.Add(new RBString("male_premium_who_splits", "The Who Split Jump Tee"));
			charTorso.Items.Add(new RBString("male_premium_who_target", "The Who Target Logo Tee"));
			charTorso.Items.Add(new RBString("militantstrappedcoat_whiteandblack", "Dr. Acula Lab Coat"));
			charTorso.Items.Add(new RBString("militaryjacketmask_canvas", "Dystopia Vest"));
			charTorso.Items.Add(new RBString("militaryjacket_canvas", "Military Issue"));
			charTorso.Items.Add(new RBString("military_wool", "Return of the Mackinaw"));
			charTorso.Items.Add(new RBString("modernsuit_cotton", "Working Man Blazer"));
			charTorso.Items.Add(new RBString("modjacketturtleneck_woolen", "Smoking Jacket"));
			charTorso.Items.Add(new RBString("modsuitjacket_sharkskin", "Sharkskin Suit"));
			charTorso.Items.Add(new RBString("motojacket_leather", "Lax Rebel Motorcycle Club"));
			charTorso.Items.Add(new RBString("napolean_cotton", "Complex Napoleon Jacket"));
			charTorso.Items.Add(new RBString("onesleevejacket_roadwarrior", "Apocophile Leathers"));
			charTorso.Items.Add(new RBString("openshirthippy_floral", "Dressed and Confused"));
			charTorso.Items.Add(new RBString("openshirthippy_silk", "Misty Mountain Top"));
			charTorso.Items.Add(new RBString("openshirt_classicfloral", "Prince of Parties"));
			charTorso.Items.Add(new RBString("oringhoodie_cotton", "O is Me Hoodie"));
			charTorso.Items.Add(new RBString("ornatecoat_bejewelled", "Bourgeois FranÃ§ois"));
			charTorso.Items.Add(new RBString("paradejacket_ornate", "Baton Jacket"));
			charTorso.Items.Add(new RBString("parka_dapper", "Mod Winter Parka"));
			charTorso.Items.Add(new RBString("plaidshirt_flannel", "The Outdoorsman"));
			charTorso.Items.Add(new RBString("polo_coatofarms", "Witticism"));
			charTorso.Items.Add(new RBString("polo_stockholmcoatofarms", "Stockholm COA"));
			charTorso.Items.Add(new RBString("punkblazer_ratty", "Blazed Out"));
			charTorso.Items.Add(new RBString("quadropheniacoat_canvas", "Jimmy's Coat"));
			charTorso.Items.Add(new RBString("roadiejacket_satin", "Roadie Jacket"));
			charTorso.Items.Add(new RBString("romantigothvest_cotton", "Byronial Underfrock"));
			charTorso.Items.Add(new RBString("ropesandcorset_blackpvc", "On the Ropes"));
			charTorso.Items.Add(new RBString("scarfandnecklaces_leaves", "Heart of Morocco"));
			charTorso.Items.Add(new RBString("scarftop_stripes", "The Libertine"));
			charTorso.Items.Add(new RBString("shearlingcoat_corduroy", "Brokeback Jacket"));
			charTorso.Items.Add(new RBString("shortlongtee_cotton", "Long and Short Tee"));
			charTorso.Items.Add(new RBString("shortlongtee_sydneykangaroo", "Aussie Kangaroo"));
			charTorso.Items.Add(new RBString("shortsleevetie_solid", "Sleeve It Alone"));
			charTorso.Items.Add(new RBString("shred_cotton", "He Shred, She Shred"));
			charTorso.Items.Add(new RBString("slashedshirt_solid", "Slasher"));
			charTorso.Items.Add(new RBString("sleevelesstee_skullprint", "Skully Sleeveless"));
			charTorso.Items.Add(new RBString("spikedarmor_leather", "Dragon Lord's Armor"));
			charTorso.Items.Add(new RBString("spikedcollar_spiked", "Cuffed and Collared"));
			charTorso.Items.Add(new RBString("spikedjacket_leather", "Spiked Twilight"));
			charTorso.Items.Add(new RBString("spikedleather_clean", "Sonic Superstate Spikes"));
			charTorso.Items.Add(new RBString("spikedshoulder_metal", "Shoulder the Burden Armor"));
			charTorso.Items.Add(new RBString("spikedtrackjacket_thinleather", "Charles the Bloody"));
			charTorso.Items.Add(new RBString("spikedvest_leather", "Troll Vest"));
			charTorso.Items.Add(new RBString("straitjacket_tattered", "Fashionable but Irrational"));
			charTorso.Items.Add(new RBString("strangejacket_plastic", "If You Don't Nomi By Now"));
			charTorso.Items.Add(new RBString("strapjacket_pleather", "Brainpower Jacket"));
			charTorso.Items.Add(new RBString("strappedshirt_pvcstraps", "Bondage Scout Uniform"));
			charTorso.Items.Add(new RBString("summonerrobe_burlap", "Summoner's Robe"));
			charTorso.Items.Add(new RBString("suspendersteeshirt_suspenders", "Braced for a Fight"));
			charTorso.Items.Add(new RBString("suspenders_studded", "Studly Suspenders"));
			charTorso.Items.Add(new RBString("sweatervest_patched", "Junk Pile Argyle"));
			charTorso.Items.Add(new RBString("tanktopscarf_unionjack", "Negative Union"));
			charTorso.Items.Add(new RBString("trackjacket_stripe", "Shoot the Runner Track Jacket"));
			charTorso.Items.Add(new RBString("tshirtbleached_cotton", "Toxic Tee"));
			charTorso.Items.Add(new RBString("tshirtbleached_eagle", "Where Eagles Dare"));
			charTorso.Items.Add(new RBString("tshirtdeath_cotton", "Death Metal Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_dragon", "The Dragonslayer"));
			charTorso.Items.Add(new RBString("tshirtdeath_dwwings", "DW Wings Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_eh", "Electro-Harmonix Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_emg", "EMG Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_ernieballlogo", "Ernie Ball Shirt"));
			charTorso.Items.Add(new RBString("tshirtdeath_fantasy", "Watercolor Fantasy"));
			charTorso.Items.Add(new RBString("tshirtdeath_ludwig", "Ludwig Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_mtchiller", "Mount Chiller"));
			charTorso.Items.Add(new RBString("tshirtdeath_pearlwings", "Pearl Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_phase", "Phase Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_rbbass", "Bass Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_rbdrums", "Drums Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_rbguitar", "Guitar Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_rbkeys", "Keys Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_rbmic", "Vox Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_ringer", "Ring Tone"));
			charTorso.Items.Add(new RBString("tshirtdeath_rockbandfour", "All Four Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_rockbandlogo", "The Rock Band Tee"));
			charTorso.Items.Add(new RBString("tshirtdeath_solid", "Solitary"));
			charTorso.Items.Add(new RBString("tshirtdeath_tiedye", "Fit to be Tie-Dyed"));
			charTorso.Items.Add(new RBString("tshirtdeath_wrestling", "High School Wrestling"));
			charTorso.Items.Add(new RBString("tshirtdeath_zvex", "Z.Vex Tee"));
			charTorso.Items.Add(new RBString("tshirt_cotton", "Favorite Cotton Tee"));
			charTorso.Items.Add(new RBString("tuxedo_sloppy", "Rat Pack Jacket"));
			charTorso.Items.Add(new RBString("twotonednbraces_blackandgrey", "Messrs Spekyll & Ryde Shirt"));
			charTorso.Items.Add(new RBString("ultimatepunk_punk", "Punk and Disorderly"));
			charTorso.Items.Add(new RBString("uniformshirt_cotton", "Delivery Dude"));
			charTorso.Items.Add(new RBString("uppcollar_denim", "Denim Demon"));
			charTorso.Items.Add(new RBString("versityjacket_leather", "Varsity Jacket"));
			charTorso.Items.Add(new RBString("vestandtee_vintage", "Vintage Vest with Tee"));
			charTorso.Items.Add(new RBString("vestdenim_canvas", "Arena Rocker"));
			charTorso.Items.Add(new RBString("vestmedallion_dragon", "Medallion Stallion"));
			charTorso.Items.Add(new RBString("vestteeth_warriors", "Brooklyn Vest"));
			charTorso.Items.Add(new RBString("vest_vest", "Vest Behavior"));
			charTorso.Items.Add(new RBString("vneckmale_cotton", "Good Guy V-neck"));
			charTorso.Items.Add(new RBString("vtank_spandex", "Wrestler Tank"));
			charTorso.Items.Add(new RBString("wifebeaterplain_biohazard", "Biohazard Boybeater"));
			charTorso.Items.Add(new RBString("wifebeater_worn", "Stella Singlet"));
			charTorso.Items.Add(new RBString("windbreaker_punked", "Savage & Patched"));
			charTorso.Items.Add(new RBString("workjacket_canvas", "Working Classy"));
			charTorso.Items.Add(new RBString("workjacket_grimey", "Pump Jockey"));
			charTorso.Items.Add(new RBString("wornlongsleeveshirt_striped", "Zoom Rugby"));
			charTorso.Items.Add(new RBString("worntshirt_vintage", "Vintage Rock Tee"));
		}

		private void AddFemFaceHair()
		{
			charFaceHair.Items.Add(new RBString("", "Bare"));
			charFaceHair.Items.Add(new RBString("female_bandana_camo", "Super Trooper"));
			charFaceHair.Items.Add(new RBString("female_bandana_cotton", "Bandito"));
			charFaceHair.Items.Add(new RBString("female_bandana_flag_ameterdam", "Dutch Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_flag_berlin", "German Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_flag_london", "UK Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_flag_mexico", "Mexican Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_flag_moscow", "Russian Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_flag_paris", "French Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_flag_reykjavik", "Icelandic Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_flag_rio", "Brazilian Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_flag_rome", "Italian Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_flag_sydney", "Aussie Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_flag_tokyo", "Japan Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_flag_usa", "USA Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_glam", "Glamma Bandana"));
			charFaceHair.Items.Add(new RBString("female_bandana_medical", "Ragin' Contagion"));
			charFaceHair.Items.Add(new RBString("female_bandana_skull", "Screamin' Skull"));
		}
		private void AddMaleFaceHair()
		{
			charFaceHair.Items.Add(new RBString("", "Bare"));
			charFaceHair.Items.Add(new RBString("male_bandana_camo", "Super Trooper"));
			charFaceHair.Items.Add(new RBString("male_bandana_cotton", "Bandito"));
			charFaceHair.Items.Add(new RBString("male_bandana_flag_ameterdam", "Dutch Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_flag_berlin", "German Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_flag_london", "UK Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_flag_mexico", "Mexican Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_flag_moscow", "Russian Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_flag_paris", "French Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_flag_reykjavik", "Icelandic Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_flag_rio", "Brazilian Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_flag_rome", "Italian Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_flag_sydney", "Aussie Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_flag_tokyo", "Japan Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_flag_usa", "USA Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_glam", "Glamma Bandana"));
			charFaceHair.Items.Add(new RBString("male_bandana_medical", "Ragin' Contagion"));
			charFaceHair.Items.Add(new RBString("male_bandana_skull", "Screamin' Skull"));
			charFaceHair.Items.Add(new RBString("male_facehair_amishbeard", "Chin Curtain"));
			charFaceHair.Items.Add(new RBString("male_facehair_bushybeard", "Full Beard"));
			charFaceHair.Items.Add(new RBString("male_facehair_bushymoustache", "Bushy 'Stache"));
			charFaceHair.Items.Add(new RBString("male_facehair_chingoatee", "Chin Beard"));
			charFaceHair.Items.Add(new RBString("male_facehair_chinstrap", "Chin Strap"));
			charFaceHair.Items.Add(new RBString("male_facehair_chops", "Lamb Chops"));
			charFaceHair.Items.Add(new RBString("male_facehair_dirtstache", "Dirt 'Stache"));
			charFaceHair.Items.Add(new RBString("male_facehair_dwarvenbeard", "Long Beard"));
			charFaceHair.Items.Add(new RBString("male_facehair_flavorsaver", "Royale"));
			charFaceHair.Items.Add(new RBString("male_facehair_ggallin", "The GG"));
			charFaceHair.Items.Add(new RBString("male_facehair_goatee", "Standard Goatee"));
			charFaceHair.Items.Add(new RBString("male_facehair_handlebar", "Chopper"));
			charFaceHair.Items.Add(new RBString("male_facehair_johnwaters", "Pencil 'Stache"));
			charFaceHair.Items.Add(new RBString("male_facehair_lemmy", "The Lemmy"));
			charFaceHair.Items.Add(new RBString("male_facehair_longgoatee", "Van Dyck"));
			charFaceHair.Items.Add(new RBString("male_facehair_moustache", "Standard 'Stache"));
			charFaceHair.Items.Add(new RBString("male_facehair_scottian", "Long Goatee"));
			charFaceHair.Items.Add(new RBString("male_facehair_shortbearde", "Short Beard"));
			charFaceHair.Items.Add(new RBString("male_facehair_shortgoatee", "Short Goatee"));
			charFaceHair.Items.Add(new RBString("male_facehair_sideburns", "Sideburns"));
		}
		private void AddGlasses()
		{
			charGlasses.Items.Add(new RBString("", "Bare"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_3dglasses_resource", "3D Glasses"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_80s_resource", "Sunglasses at Night"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_aviators_resource", "Miranda Sunglasses"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_badass_resource", "Agent Issue"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_blinds_resource", "Blinded by the Light Shades"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_bugeyes_resource", "Bug Eyes"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_business_resource", "Business Class Glasses"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_cataracts_resource", "Bluff Shades"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_catseye_resource", "Librarian's"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_circles_resource", "Peace & Love Sunnies"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_costellos_resource", "Costellos"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_eyepatch_resource", "Pirate Eye-Patch"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_goggles_resource", "Steampunk Vision"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_jackieos_resource", "Jackie O's"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_macho_resource", "K-12s"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_mod_resource", "Tres Moderne Sunglasses"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_monocle_resource", "Sterling Reputation"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_newwaves_resource", "Neo Maxi Zoom Dweebie"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_rectangle_resource", "Wrecktangles"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_smashed_resource", "Four Eyes"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_wayfarers_resource", "Route 66 Shades"));
			charGlasses.Items.Add(new RBString(genderString + "glasses_wireframe_resource", "Wire Frame Glasses"));
			charGlasses.Items.Add(new RBString(genderString + "mask_cat_leopard_resource", "Deft Leopard"));
			charGlasses.Items.Add(new RBString(genderString + "mask_cat_tiger_resource", "Face of the Tiger"));
			charGlasses.Items.Add(new RBString(genderString + "mask_eyebandage_resource", "First Eye Blind"));
			charGlasses.Items.Add(new RBString(genderString + "mask_gasmaskstudded_resource", "Spiked Gas Mask"));
			charGlasses.Items.Add(new RBString(genderString + "mask_gasmask_resource", "Jumpin' Flak Mask"));
			charGlasses.Items.Add(new RBString(genderString + "mask_hockey_resource", "Hockey Mask"));
			charGlasses.Items.Add(new RBString(genderString + "mask_horse_domestic_resource", "Horse with No Name"));
			charGlasses.Items.Add(new RBString(genderString + "mask_horse_zebra_resource", "The Zebra"));
			charGlasses.Items.Add(new RBString(genderString + "mask_loneranger_leather_resource", "Stitched in Time"));
			charGlasses.Items.Add(new RBString(genderString + "mask_loneranger_resource", "Unknown Stranger"));
			charGlasses.Items.Add(new RBString(genderString + "mask_masque_resource", "Black Masquerade"));
			charGlasses.Items.Add(new RBString(genderString + "mask_pig_resource", "Oi! Oi! Oink!"));
			charGlasses.Items.Add(new RBString(genderString + "mask_plaguedoctor_resource", "Plague Doctor"));
			charGlasses.Items.Add(new RBString(genderString + "mask_skull_resource", "Numb Skull"));
			charGlasses.Items.Add(new RBString(genderString + "mask_sugarskull_classic_resource", "Sugar Skull"));
			charGlasses.Items.Add(new RBString(genderString + "mask_sugarskull_fandango_resource", "Fandango Skull"));
			charGlasses.Items.Add(new RBString(genderString + "mask_sugarskull_resource", "Day of the Dead"));
		}
		private void AddFemHair()
		{
			charHair.Items.Add(new RBString("", "Bald"));
			charHair.Items.Add(new RBString("female_hair_50sbandana_resource", "Retro Pony"));
			charHair.Items.Add(new RBString("female_hair_70sbisset_resource", "The Jacqueline"));
			charHair.Items.Add(new RBString("female_hair_beehive_resource", "Beehive"));
			charHair.Items.Add(new RBString("female_hair_bettybangs_resource", "The Betty"));
			charHair.Items.Add(new RBString("female_hair_blownback_resource", "Dolled Up"));
			charHair.Items.Add(new RBString("female_hair_bunhead_resource", "Princess Player"));
			charHair.Items.Add(new RBString("female_hair_buzzcut_resource", "Buzz Cut"));
			charHair.Items.Add(new RBString("female_hair_cutepigtails_resource", "Little Pigtails"));
			charHair.Items.Add(new RBString("female_hair_dreadpony_resource", "...And You Will Know Us By the Tail of Dreads"));
			charHair.Items.Add(new RBString("female_hair_emobuzz_resource", "Emo Philia"));
			charHair.Items.Add(new RBString("female_hair_farah_resource", "Angel Wings"));
			charHair.Items.Add(new RBString("female_hair_feathered_short_resource", "Feathered"));
			charHair.Items.Add(new RBString("female_hair_femover_resource", "Femover"));
			charHair.Items.Add(new RBString("female_hair_flapper_resource", "The Flapper"));
			charHair.Items.Add(new RBString("female_hair_gothdreads_resource", "Dread Falls"));
			charHair.Items.Add(new RBString("female_hair_hilldreads_resource", "Free Locks"));
			charHair.Items.Add(new RBString("female_hair_hippybangs_resource", "Hippie Bangs"));
			charHair.Items.Add(new RBString("female_hair_hornhair_resource", "Puck It Up"));
			charHair.Items.Add(new RBString("female_hair_japan_resource", "Hairajuku"));
			charHair.Items.Add(new RBString("female_hair_ladyfrosmall_resource", "T.W.A."));
			charHair.Items.Add(new RBString("female_hair_ladyfro_resource", "The Natural"));
			charHair.Items.Add(new RBString("female_hair_ladylayered_resource", "Layered"));
			charHair.Items.Add(new RBString("female_hair_ladypunk_resource", "Punked Out"));
			charHair.Items.Add(new RBString("female_hair_lolita_resource", "Lolita"));
			charHair.Items.Add(new RBString("female_hair_longstraight_resource", "Long and Straight"));
			charHair.Items.Add(new RBString("female_hair_longwavy_resource", "Long and Wavy"));
			charHair.Items.Add(new RBString("female_hair_long_resource", "Long Hair"));
			charHair.Items.Add(new RBString("female_hair_loosepony_resource", "Casual Hipster"));
			charHair.Items.Add(new RBString("female_hair_messyshort_resource", "Hot Mess"));
			charHair.Items.Add(new RBString("female_hair_mulletbob_resource", "Lady Valiant"));
			charHair.Items.Add(new RBString("female_hair_nightcat_resource", "Nightcat"));
			charHair.Items.Add(new RBString("female_hair_oddbob_resource", "Thingama Bob"));
			charHair.Items.Add(new RBString("female_hair_pigtails_resource", "Long Pigtails"));
			charHair.Items.Add(new RBString("female_hair_pixie_resource", "The Pixie"));
			charHair.Items.Add(new RBString("female_hair_pulledback_resource", "Pulled Back"));
			charHair.Items.Add(new RBString("female_hair_reverseflip_resource", "Reverse Flip"));
			charHair.Items.Add(new RBString("female_hair_shaggy_resource", "Shaggy Hair"));
			charHair.Items.Add(new RBString("female_hair_shortbangs_resource", "Pageboy"));
			charHair.Items.Add(new RBString("female_hair_shortspikes_resource", "Short Spikes"));
			charHair.Items.Add(new RBString("female_hair_short_resource", "Chopped-Off"));
			charHair.Items.Add(new RBString("female_hair_sideflip_resource", "Asymmetrical Bob"));
			charHair.Items.Add(new RBString("female_hair_sidepony_resource", "Side Ponytail"));
			charHair.Items.Add(new RBString("female_hair_spikyteased_resource", "The Joan"));
			charHair.Items.Add(new RBString("female_hair_test", "Waves of Mutilation"));
			charHair.Items.Add(new RBString("female_hair_trihawk_resource", "Trihawk"));
			charHair.Items.Add(new RBString("female_hair_visor_resource", "Contrast Fringe"));
			charHair.Items.Add(new RBString("female_hair_youngozzy_resource", "Curtained"));
			charHair.Items.Add(new RBString("female_hat_bowler_messyshort_resource", "Holy Bowler"));
			charHair.Items.Add(new RBString("female_hat_bowler_short_resource", "Holy Bowler w/ Short Hair"));
			charHair.Items.Add(new RBString("female_hat_brete_longwavy_resource", "Pardon Your French w/ Long Hair"));
			charHair.Items.Add(new RBString("female_hat_brete_shaggybob_resource", "Pardon Your French w/ Short Hair"));
			charHair.Items.Add(new RBString("female_hat_brete_solid", "Pardon Your French"));
			charHair.Items.Add(new RBString("female_hat_cophat_longhair_resource", "Biker Cap"));
			charHair.Items.Add(new RBString("female_hat_cowboyhat_ladylayered_resource", "Urban Cowboy"));
			charHair.Items.Add(new RBString("female_hat_dockcap_cutepigtails_resource", "Knit Cap"));
			charHair.Items.Add(new RBString("female_hat_duckbill_femover_resource", "Raspberry Cap"));
			charHair.Items.Add(new RBString("female_hat_duckbill_ladylayered_resource", "Fancy Newsboy"));
			charHair.Items.Add(new RBString("female_hat_fedora_flapper_resource", "Fab Fedora"));
			charHair.Items.Add(new RBString("female_hat_fedora_longstraight_resource", "Suburban Gangster"));
			charHair.Items.Add(new RBString("female_hat_femgothveil_flapper_resource", "Mourning Glory Veil"));
			charHair.Items.Add(new RBString("female_hat_gothveil_pixie_resource", "Lolita Hat"));
			charHair.Items.Add(new RBString("female_hat_gothviel_bunhead_resource", "Angel of the Mourning"));
			charHair.Items.Add(new RBString("female_hat_greenvisor_shortspikes_resource", "Super Visor"));
			charHair.Items.Add(new RBString("female_hat_hippybangs_maohat_resource", "Chairman's Cap"));
			charHair.Items.Add(new RBString("female_hat_militarycap_loosepony_resource", "Tough Girl"));
			charHair.Items.Add(new RBString("female_hat_militarycap_shortspikes_resource", "G.I. Jane"));
			charHair.Items.Add(new RBString("female_hat_outlaw_farrah_resource", "Metal Outlaw w/ Long Hair"));
			charHair.Items.Add(new RBString("female_hat_outlaw_shortspikes_resource", "Metal Outlaw w/ Short Hair"));
			charHair.Items.Add(new RBString("female_hat_rockheaddress_resource", "Head-turning Headdress"));
			charHair.Items.Add(new RBString("female_hat_rockhelmet_eagle_resource", "Hell on Wheels"));
			charHair.Items.Add(new RBString("female_hat_rockhelmet_nogoggles_resource", "Give 'Em Helmet"));
			charHair.Items.Add(new RBString("female_hat_rockhelmet_resource", "Speed Demon"));
			charHair.Items.Add(new RBString("female_hat_rockhelmet_stripes_resource", "Stripe Tease Helmet"));
			charHair.Items.Add(new RBString("female_hat_rockhelmet_wings_resource", "Worn in the USA Helmet"));
			charHair.Items.Add(new RBString("female_hat_spikehelm_shortspikes_resource", "Pickelhaube"));
			charHair.Items.Add(new RBString("female_hat_stewardesshat_sidepony_resource", "Friendly Skies Cap"));
			charHair.Items.Add(new RBString("female_hat_tam_dreadpony_resource", "Knock 'Em Dreads"));
			charHair.Items.Add(new RBString("female_hat_tam_longwavy_resource", "Rasta Mama"));
			charHair.Items.Add(new RBString("female_hat_tophat_sideflip_resource", "Top Hat"));
			charHair.Items.Add(new RBString("female_hat_trucker_70sbisset_resource", "Irony Cap"));
			charHair.Items.Add(new RBString("female_hat_wolfhead_hairy", "Hungry Like the Wolf"));
		}
		private void AddMaleHair()
		{
			charHair.Items.Add(new RBString("", "Bald"));
			charHair.Items.Add(new RBString("male_hair_baldeagle_resource", "Bald Eagle"));
			charHair.Items.Add(new RBString("male_hair_bandanna_resource", "Stray Dog"));
			charHair.Items.Add(new RBString("male_hair_bedhead_resource", "Bed Head"));
			charHair.Items.Add(new RBString("male_hair_billyidol_resource", "Short Punk"));
			charHair.Items.Add(new RBString("male_hair_buzzcut_resource", "Buzz Cut"));
			charHair.Items.Add(new RBString("male_hair_crazyhawk_resource", "Crazyhawk"));
			charHair.Items.Add(new RBString("male_hair_dreadpony_resource", "...And You Will Know Us By the Tail of Dreads"));
			charHair.Items.Add(new RBString("male_hair_egyptian_resource", "Sleek Mop"));
			charHair.Items.Add(new RBString("male_hair_emobuzz_resource", "Emo Philia"));
			charHair.Items.Add(new RBString("male_hair_emover_resource", "The Emover"));
			charHair.Items.Add(new RBString("male_hair_fauxhawk_resource", "Fauxhawk"));
			charHair.Items.Add(new RBString("male_hair_featheredshort_resource", "Feathered"));
			charHair.Items.Add(new RBString("male_hair_fropick_resource", "Afro"));
			charHair.Items.Add(new RBString("male_hair_generic_resource", "Totally Generic"));
			charHair.Items.Add(new RBString("male_hair_gentleman_resource", "Dapper Dan (Left Part)"));
			charHair.Items.Add(new RBString("male_hair_gentleman_rightpart_resource", "Dapper Dan (Right Part)"));
			charHair.Items.Add(new RBString("male_hair_glampoof_resource", "Glamtastic"));
			charHair.Items.Add(new RBString("male_hair_greaserslick_resource", "Greaser"));
			charHair.Items.Add(new RBString("male_hair_greaser_resource", "The Pompadour"));
			charHair.Items.Add(new RBString("male_hair_hairhorns_resource", "Robin Badfellow"));
			charHair.Items.Add(new RBString("male_hair_hanoi2_resource", "Hanoi Locks"));
			charHair.Items.Add(new RBString("male_hair_hanoi_resource", "Hair Metal"));
			charHair.Items.Add(new RBString("male_hair_lazyhawk_resource", "Lazyhawk"));
			charHair.Items.Add(new RBString("male_hair_libertyspikes_resource", "The Quill Pig"));
			charHair.Items.Add(new RBString("male_hair_longlayered_resource", "Long and Layered"));
			charHair.Items.Add(new RBString("male_hair_longmop_resource", "Boy Toy"));
			charHair.Items.Add(new RBString("male_hair_longwavy_resource", "Long and Wavy"));
			charHair.Items.Add(new RBString("male_hair_long_resource", "Long Hair"));
			charHair.Items.Add(new RBString("male_hair_maleladyfro_resource", "The Natural"));
			charHair.Items.Add(new RBString("male_hair_mandana_resource", "Mandana"));
			charHair.Items.Add(new RBString("male_hair_messymop_resource", "Messy Mop"));
			charHair.Items.Add(new RBString("male_hair_mohawk_resource", "Mohawk"));
			charHair.Items.Add(new RBString("male_hair_mop_resource", "Classic Mop"));
			charHair.Items.Add(new RBString("male_hair_nightcat_resource", "Nightcat"));
			charHair.Items.Add(new RBString("male_hair_parkinglot_resource", "Tailgater"));
			charHair.Items.Add(new RBString("male_hair_powermullet_resource", "Power Mullet"));
			charHair.Items.Add(new RBString("male_hair_prettyboy_resource", "Pretty Boy"));
			charHair.Items.Add(new RBString("male_hair_ramones_resource", "Gabba Gabba Hair!"));
			charHair.Items.Add(new RBString("male_hair_receded_resource", "Most Likely to Recede"));
			charHair.Items.Add(new RBString("male_hair_robertplant_resource", "Long Twists"));
			charHair.Items.Add(new RBString("male_hair_shaggy_resource", "Shaggy Hair"));
			charHair.Items.Add(new RBString("male_hair_shortcurly_resource", "Curls Just Wanna Have Fun"));
			charHair.Items.Add(new RBString("male_hair_shortspikes_resource", "Short Spikes"));
			charHair.Items.Add(new RBString("male_hair_squeebpunk_resource", "Squeeb"));
			charHair.Items.Add(new RBString("male_hair_younghr_resource", "Better Off Dreads"));
			charHair.Items.Add(new RBString("male_hair_youngozzy_resource", "Curtained"));
			charHair.Items.Add(new RBString("male_hair_ziggymullet_resource", "Ziggy's Mullet"));
			charHair.Items.Add(new RBString("male_hat_armyhelmet_fauxhawk_resource", "Army Helmet w/ Short Hair"));
			charHair.Items.Add(new RBString("male_hat_armyhelmet_nightcat_resource", "Army Helmet w/ Long Hair"));
			charHair.Items.Add(new RBString("male_hat_armyhelmet_resource", "You and What Army Helmet"));
			charHair.Items.Add(new RBString("male_hat_aviatorcap_parkinglot_resource", "Aviator Cap"));
			charHair.Items.Add(new RBString("male_hat_bowler_fauxhawk_resource", "Holy Bowler w/ Short Hair"));
			charHair.Items.Add(new RBString("male_hat_bowler_resource", "Holy Bowler"));
			charHair.Items.Add(new RBString("male_hat_cophat_fauxhawk_resource", "Biker Cap"));
			charHair.Items.Add(new RBString("male_hat_cophat_longlayered_resource", "Biker Cap w/ Long Hair"));
			charHair.Items.Add(new RBString("male_hat_cophat_resource", "Wool Bully Cap"));
			charHair.Items.Add(new RBString("male_hat_cowboy_fauxhawk_resource", "Happy Rancher"));
			charHair.Items.Add(new RBString("male_hat_cowboy_long_resource", "Suburban Cowboy"));
			charHair.Items.Add(new RBString("male_hat_cowboy_ziggymullet_resource", "Urban Cowboy"));
			charHair.Items.Add(new RBString("male_hat_dockcap_shaggy_resource", "Knit Cap"));
			charHair.Items.Add(new RBString("male_hat_fedora_gentleman_resource", "The Humphrey"));
			charHair.Items.Add(new RBString("male_hat_fedora_youngozzie_resource", "Jazz Man"));
			charHair.Items.Add(new RBString("male_hat_footballhelmet_decal", "Armchair Athlete"));
			charHair.Items.Add(new RBString("male_hat_footballhelmet_long_resource", "Armchair Athlete w/ Long Hair"));
			charHair.Items.Add(new RBString("male_hat_greenvisor_baldeagle_resource", "Super Visor w/ Short Hair"));
			charHair.Items.Add(new RBString("male_hat_helmet_ramones_resource", "Hell on Wheels"));
			charHair.Items.Add(new RBString("male_hat_malemetalhead_resource", "Beast of Burden Head"));
			charHair.Items.Add(new RBString("male_hat_maohat_parkinglot_resource", "Chairman's Cap"));
			charHair.Items.Add(new RBString("male_hat_militarycap_fauxhawk_resource", "G.I. Cap"));
			charHair.Items.Add(new RBString("male_hat_militarycap_featheredshort_resource", "Bryn Bonnet"));
			charHair.Items.Add(new RBString("male_hat_outlaw_fauxhawk_resource", "The Bad Guy w/ Short Hair"));
			charHair.Items.Add(new RBString("male_hat_outlaw_resource", "The Bad Guy"));
			charHair.Items.Add(new RBString("male_hat_paperboy_fauxhawk_resource", "Newsie"));
			charHair.Items.Add(new RBString("male_hat_paperboy_long_resource", "Paperboy"));
			charHair.Items.Add(new RBString("male_hat_rockhat_long_resource", "Rhinestone Cowboy Hat"));
			charHair.Items.Add(new RBString("male_hat_rockhelmet_nogoggles_resource", "Give 'Em Helmet"));
			charHair.Items.Add(new RBString("male_hat_spikehelm_crazyhawk_resource", "Pickelhaube"));
			charHair.Items.Add(new RBString("male_hat_tam_dreadpony_resource", "Jamaican Me Crazy"));
			charHair.Items.Add(new RBString("male_hat_tam_maleladyfro_resource", "Dread Wrangler"));
			charHair.Items.Add(new RBString("male_hat_tophat_long_resource", "Top Hat"));
			charHair.Items.Add(new RBString("male_hat_trucker_long_resource", "Redneck"));
			charHair.Items.Add(new RBString("male_hat_trucker_shaggy_resource", "The Irony Cap"));
			charHair.Items.Add(new RBString("male_hat_trucker_ziggymullet_resource", "Backwards Irony Cap"));
			charHair.Items.Add(new RBString("male_hat_viking_long_resource", "Norseman"));
			charHair.Items.Add(new RBString("male_hat_viking_resource", "Erik the Rad"));
			charHair.Items.Add(new RBString("male_hat_ziggymullet_greenvisor_resource", "Super Visor w/ Long Hair"));
		}
		private void AddFemWrist()
		{
			charWrist.Items.Add(new RBString("", "Bare"));
			charWrist.Items.Add(new RBString("femalewrist_avrilsock", "Stripy Wristwarmer"));
			charWrist.Items.Add(new RBString("femalewrist_barbedwire", "Devil's Rope Bracelets"));
			charWrist.Items.Add(new RBString("femalewrist_barbedwire_right", "Devil's Rope (Single)"));
			charWrist.Items.Add(new RBString("femalewrist_checkerpyramids", "Checked Off Cuffs"));
			charWrist.Items.Add(new RBString("femalewrist_checkerpyramids_right", "Checked Off Cuff (Single)"));
			charWrist.Items.Add(new RBString("femalewrist_eagle", "Worn in the USA Bracer"));
			charWrist.Items.Add(new RBString("femalewrist_gauntlet", "Buckled Gauntlets"));
			charWrist.Items.Add(new RBString("femalewrist_gauntlet2", "Spiked Gauntlet"));
			charWrist.Items.Add(new RBString("femalewrist_gauntlet_right", "Buckled Gauntlet (Single)"));
			charWrist.Items.Add(new RBString("femalewrist_hercules", "Hero's Cuffs"));
			charWrist.Items.Add(new RBString("femalewrist_hercules_right", "Hero's Cuff (Single)"));
			charWrist.Items.Add(new RBString("femalewrist_jelly", "Jelly Bracelets"));
			charWrist.Items.Add(new RBString("femalewrist_jelly_right", "Jelly Bracelet (Single)"));
			charWrist.Items.Add(new RBString("femalewrist_laceduparm", "Laced Down"));
			charWrist.Items.Add(new RBString("femalewrist_longspikes", "Yard Dog Cuffs"));
			charWrist.Items.Add(new RBString("femalewrist_longspikes_right", "Yard Dog Cuff (Single)"));
			charWrist.Items.Add(new RBString("femalewrist_nailgauntlet", "Brad the Impaler Gauntlet"));
			charWrist.Items.Add(new RBString("femalewrist_pyramids", "Pyramid Spiked Cuffs"));
			charWrist.Items.Add(new RBString("femalewrist_starstuds", "Stars and Studs"));
			charWrist.Items.Add(new RBString("femalewrist_studdedbracer", "Studded Bracer"));
			charWrist.Items.Add(new RBString("femalewrist_studdedbracer_right", "Studded Bracer (Single)"));
			charWrist.Items.Add(new RBString("femalewrist_studdedgauntlet", "Studded Gauntlets"));
			charWrist.Items.Add(new RBString("femalewrist_studdedgauntlet_right", "Studded Gauntlet (Single)"));
			charWrist.Items.Add(new RBString("femalewrist_studdedwatchcombo", "Good Times"));
			charWrist.Items.Add(new RBString("femalewrist_studdedwatchcombo_right", "Good Time (Single)"));
			charWrist.Items.Add(new RBString("femalewrist_sweatband", "Perspicacity"));
			charWrist.Items.Add(new RBString("femalewrist_sweatbandcharmscombo", "Sweet & Tart"));
			charWrist.Items.Add(new RBString("femalewrist_sweatbandcombo", "Perspiration & Pyramids"));
			charWrist.Items.Add(new RBString("femalewrist_sweatbandstuddedcombo", "Sweat & Spikes"));
			charWrist.Items.Add(new RBString("femalewrist_sweatband_right", "Perspicacity (Single)"));
			charWrist.Items.Add(new RBString("femalewrist_watchleaterstrap", "Time to Party Watch"));
			charWrist.Items.Add(new RBString("femalewrist_wristbandterrycloth", "Wristwarmers"));
			charWrist.Items.Add(new RBString("femalewrist_wristbandterrycloth_right", "Wristwarmer (Single)"));
		}
		private void AddMaleWrist()
		{
			charWrist.Items.Add(new RBString("", "Bare"));
			charWrist.Items.Add(new RBString("cast_graffiti", "Cast-anova"));
			charWrist.Items.Add(new RBString("malewrist_avrilsock", "Stripy Wristwarmer"));
			charWrist.Items.Add(new RBString("malewrist_barbedwire", "Devil's Rope Bracelets"));
			charWrist.Items.Add(new RBString("malewrist_barbedwire_right", "Devil's Rope (Single)"));
			charWrist.Items.Add(new RBString("malewrist_checkerpyramids", "Checked Off Cuffs"));
			charWrist.Items.Add(new RBString("malewrist_checkerpyramids_right", "Checked Off Cuff (Single)"));
			charWrist.Items.Add(new RBString("malewrist_gauntlet", "Buckled Gauntlets"));
			charWrist.Items.Add(new RBString("malewrist_gauntlet2", "Spiked Gauntlet"));
			charWrist.Items.Add(new RBString("malewrist_gauntlet_right", "Buckled Gauntlet (Single)"));
			charWrist.Items.Add(new RBString("malewrist_hercules", "Hero's Cuffs"));
			charWrist.Items.Add(new RBString("malewrist_hercules_right", "Hero's Cuff (Single)"));
			charWrist.Items.Add(new RBString("malewrist_laceduparm", "Laced Down"));
			charWrist.Items.Add(new RBString("malewrist_leatherstrap", "Buried in Leather"));
			charWrist.Items.Add(new RBString("malewrist_longspikes", "Yard Dog Cuffs"));
			charWrist.Items.Add(new RBString("malewrist_longspikes_right", "Yard Dog Cuff (Single)"));
			charWrist.Items.Add(new RBString("malewrist_nailgauntlet", "Brad the Impaler Gauntlet"));
			charWrist.Items.Add(new RBString("malewrist_pyramids", "Pyramid Spiked Cuffs"));
			charWrist.Items.Add(new RBString("malewrist_studdedbracer", "Studded Bracer"));
			charWrist.Items.Add(new RBString("malewrist_studdedgauntlet", "Studded Gauntlets"));
			charWrist.Items.Add(new RBString("malewrist_studdedgauntlet_right", "Studded Gauntlet (Single)"));
			charWrist.Items.Add(new RBString("malewrist_studdedwatchcombo", "Good Times"));
			charWrist.Items.Add(new RBString("malewrist_studdedwatchcombo_right", "Good Time (Single)"));
			charWrist.Items.Add(new RBString("malewrist_sweatband", "Perspicacity"));
			charWrist.Items.Add(new RBString("malewrist_sweatbandcombo", "Perspiration & Pyramids"));
			charWrist.Items.Add(new RBString("malewrist_sweatbandcombo_right", "Sweatband Collector"));
			charWrist.Items.Add(new RBString("malewrist_sweatbandstuddedcombo", "Sweat & Spikes"));
			charWrist.Items.Add(new RBString("malewrist_sweatband_right", "Perspicacity (Single)"));
			charWrist.Items.Add(new RBString("malewrist_watchcalculator", "Calcu-Watch Xt4000"));
			charWrist.Items.Add(new RBString("malewrist_watchleatherstrap", "Time to Party Watch"));
			charWrist.Items.Add(new RBString("malewrist_wristbandterrycloth", "Wristwarmers"));
			charWrist.Items.Add(new RBString("malewrist_wristbandterrycloth_right", "Wristwarmer (Single)"));
		}

		private void AddGuitar()
		{
			charGuitar.Items.Add(new RBString("51squier_paint", "Squier '51, Paint"));
			charGuitar.Items.Add(new RBString("51squier_sparkle", "Squier '51, Sparkle"));
			charGuitar.Items.Add(new RBString("51squier_sunburstblack", "Squier '51, Black Sunburst"));
			charGuitar.Items.Add(new RBString("51squier_sunburstpearl", "Squier '51, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("51squier_sunbursttortoise", "Squier '51, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("51squier_sunburstwhite", "Squier '51, White Sunburst"));
			charGuitar.Items.Add(new RBString("51squier_triburstblack", "Squier '51, Black Triburst"));
			charGuitar.Items.Add(new RBString("51squier_triburstpearl", "Squier '51, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("51squier_tribursttortoise", "Squier '51, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("51squier_triburstwhite", "Squier '51, White Triburst"));
			charGuitar.Items.Add(new RBString("51squier_woodash", "Squier '51, Ash"));
			charGuitar.Items.Add(new RBString("51squier_woodmaple", "Squier '51, Maple"));
			charGuitar.Items.Add(new RBString("astrojet_paint", "Gretsch Astro-Jet w/ Vibrato Tailpiece, Paint"));
			charGuitar.Items.Add(new RBString("astrojet_sparkle", "Gretsch Astro-Jet w/ Vibrato Tailpiece, Sparkle"));
			charGuitar.Items.Add(new RBString("astrojet_sunburstblack", "Gretsch Astro-Jet w/ Vibrato Tailpiece, Black Sunburst"));
			charGuitar.Items.Add(new RBString("astrojet_sunburstpearl", "Gretsch Astro-Jet w/ Vibrato Tailpiece, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("astrojet_sunbursttortoise", "Gretsch Astro-Jet w/ Vibrato Tailpiece, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("astrojet_sunburstwhite", "Gretsch Astro-Jet w/ Vibrato Tailpiece, White Sunburst"));
			charGuitar.Items.Add(new RBString("astrojet_triburstblack", "Gretsch Astro-Jet w/ Vibrato Tailpiece, Black Triburst"));
			charGuitar.Items.Add(new RBString("astrojet_triburstpearl", "Gretsch Astro-Jet w/ Vibrato Tailpiece, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("astrojet_tribursttortoise", "Gretsch Astro-Jet w/ Vibrato Tailpiece, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("astrojet_triburstwhite", "Gretsch Astro-Jet w/ Vibrato Tailpiece, White Triburst"));
			charGuitar.Items.Add(new RBString("astrojet_woodash", "Gretsch Astro-Jet w/ Vibrato Tailpiece, Ash"));
			charGuitar.Items.Add(new RBString("astrojet_woodmaple", "Gretsch Astro-Jet w/ Vibrato Tailpiece, Maple"));
			charGuitar.Items.Add(new RBString("axe_resource", "The Axecutioner"));
			charGuitar.Items.Add(new RBString("batwing_resource", "The Batwing"));
			charGuitar.Items.Add(new RBString("bodiddley_resource", "Gretsch G6138 Bo Diddley"));
			charGuitar.Items.Add(new RBString("bolt_resource", "Lightning Bolt!"));
			charGuitar.Items.Add(new RBString("brain_resource", "The Brain"));
			charGuitar.Items.Add(new RBString("chainsaw_resource", "The Chainsaw"));
			charGuitar.Items.Add(new RBString("committee_paint", "Gretsch Committee, Paint"));
			charGuitar.Items.Add(new RBString("committee_sparkle", "Gretsch Committee, Sparkle"));
			charGuitar.Items.Add(new RBString("committee_sunburst", "Gretsch Committee, Sunburst"));
			charGuitar.Items.Add(new RBString("committee_triburst", "Gretsch Committee, Triburst"));
			charGuitar.Items.Add(new RBString("committee_woodash", "Gretsch Committee, Ash"));
			charGuitar.Items.Add(new RBString("committee_woodmaple", "Gretsch Committee, Maple"));
			charGuitar.Items.Add(new RBString("corvette_paint", "Gretsch G5135 Electromatic Corvette, Paint"));
			charGuitar.Items.Add(new RBString("corvette_sparkle", "Gretsch G5135 Electromatic Corvette, Sparkle"));
			charGuitar.Items.Add(new RBString("corvette_sunburstblack", "Gretsch G5135 Electromatic Corvette, Black Sunburst"));
			charGuitar.Items.Add(new RBString("corvette_sunburstpearl", "Gretsch G5135 Electromatic Corvette, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("corvette_sunbursttortoise", "Gretsch G5135 Electromatic Corvette, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("corvette_sunburstwhite", "Gretsch G5135 Electromatic Corvette, White Sunburst"));
			charGuitar.Items.Add(new RBString("corvette_triburstblack", "Gretsch G5135 Electromatic Corvette, Black Triburst"));
			charGuitar.Items.Add(new RBString("corvette_triburstpearl", "Gretsch G5135 Electromatic Corvette, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("corvette_tribursttortoise", "Gretsch G5135 Electromatic Corvette, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("corvette_triburstwhite", "Gretsch G5135 Electromatic Corvette, White Triburst"));
			charGuitar.Items.Add(new RBString("corvette_woodash", "Gretsch G5135 Electromatic Corvette, Ash"));
			charGuitar.Items.Add(new RBString("corvette_woodmaple", "Gretsch G5135 Electromatic Corvette, Maple"));
			charGuitar.Items.Add(new RBString("cthulhu_resource", "Cthulhu's Revenge"));
			charGuitar.Items.Add(new RBString("d2010_paint", "Ovation D2010, Paint"));
			charGuitar.Items.Add(new RBString("d2010_sparkle", "Ovation D2010, Sparkle"));
			charGuitar.Items.Add(new RBString("d2010_sunburstblack", "Ovation D2010, Black Sunburst"));
			charGuitar.Items.Add(new RBString("d2010_sunburstpearl", "Ovation D2010, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("d2010_sunbursttortoise", "Ovation D2010, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("d2010_sunburstwhite", "Ovation D2010, White Sunburst"));
			charGuitar.Items.Add(new RBString("d2010_triburstblack", "Ovation D2010, Black Triburst"));
			charGuitar.Items.Add(new RBString("d2010_triburstpearl", "Ovation D2010, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("d2010_tribursttortoise", "Ovation D2010, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("d2010_triburstwhite", "Ovation D2010, White Triburst"));
			charGuitar.Items.Add(new RBString("d2010_woodash", "Ovation D2010, Ash"));
			charGuitar.Items.Add(new RBString("d2010_woodmaple", "Ovation D2010, Maple"));
			charGuitar.Items.Add(new RBString("dinky01_paint", "Jackson DK1 Dinky, Paint"));
			charGuitar.Items.Add(new RBString("dinky01_sparkle", "Jackson DK1 Dinky, Sparkle"));
			charGuitar.Items.Add(new RBString("dinky01_sunburst", "Jackson DK1 Dinky, Sunburst"));
			charGuitar.Items.Add(new RBString("dinky01_triburst", "Jackson DK1 Dinky, Triburst"));
			charGuitar.Items.Add(new RBString("dinky01_woodash", "Jackson DK1 Dinky, Ash"));
			charGuitar.Items.Add(new RBString("dinky01_woodmaple", "Jackson DK1 Dinky, Maple"));
			charGuitar.Items.Add(new RBString("doublejet_paint", "Gretsch G5248T Electromatic Double Jet, Paint"));
			charGuitar.Items.Add(new RBString("doublejet_sparkle", "Gretsch G5248T Electromatic Double Jet, Sparkle"));
			charGuitar.Items.Add(new RBString("doublejet_sunburstblack", "Gretsch G5248T Electromatic Double Jet, Black Sunburst"));
			charGuitar.Items.Add(new RBString("doublejet_sunburstpearl", "Gretsch G5248T Electromatic Double Jet, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("doublejet_sunbursttortoise", "Gretsch G5248T Electromatic Double Jet, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("doublejet_sunburstwhite", "Gretsch G5248T Electromatic Double Jet, White Sunburst"));
			charGuitar.Items.Add(new RBString("doublejet_triburstblack", "Gretsch G5248T Electromatic Double Jet, Black Triburst"));
			charGuitar.Items.Add(new RBString("doublejet_triburstpearl", "Gretsch G5248T Electromatic Double Jet, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("doublejet_tribursttortoise", "Gretsch G5248T Electromatic Double Jet, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("doublejet_triburstwhite", "Gretsch G5248T Electromatic Double Jet, White Triburst"));
			charGuitar.Items.Add(new RBString("doublejet_woodash", "Gretsch G5248T Electromatic Double Jet, Ash"));
			charGuitar.Items.Add(new RBString("doublejet_woodmaple", "Gretsch G5248T Electromatic Double Jet, Maple"));
			charGuitar.Items.Add(new RBString("esprit_paint", "Squier Esprit, Paint"));
			charGuitar.Items.Add(new RBString("esprit_sparkle", "Squier Esprit, Sparkle"));
			charGuitar.Items.Add(new RBString("esprit_stop", "Squier Esprit, Stop"));
			charGuitar.Items.Add(new RBString("esprit_sunburst", "Squier Esprit, Sunburst"));
			charGuitar.Items.Add(new RBString("esprit_triburst", "Squier Esprit, Triburst"));
			charGuitar.Items.Add(new RBString("esprit_woodash", "Squier Esprit, Ash"));
			charGuitar.Items.Add(new RBString("esprit_woodmaple", "Squier Esprit, Maple"));
			charGuitar.Items.Add(new RBString("falcon_paint", "Gretsch G6136T White Falcon"));
			charGuitar.Items.Add(new RBString("g5120_paint", "Gretsch G5120 Electromatic Hollow Body, Paint"));
			charGuitar.Items.Add(new RBString("g5120_sparkle", "Gretsch G5120 Electromatic Hollow Body, Sparkle"));
			charGuitar.Items.Add(new RBString("g5120_sunburst", "Gretsch G5120 Electromatic Hollow Body, Sunburst"));
			charGuitar.Items.Add(new RBString("g5120_triburst", "Gretsch G5120 Electromatic Hollow Body, Triburst"));
			charGuitar.Items.Add(new RBString("g5120_woodash", "Gretsch G5120 Electromatic Hollow Body, Ash"));
			charGuitar.Items.Add(new RBString("g5120_woodmaple", "Gretsch G5120 Electromatic Hollow Body, Maple"));
			charGuitar.Items.Add(new RBString("goathead_resource", "The Goat Head"));
			charGuitar.Items.Add(new RBString("greenday_blue", "Green Day Guitar"));
			charGuitar.Items.Add(new RBString("hbomb_resource", "The Bomb"));
			charGuitar.Items.Add(new RBString("jaguar01_paint", "Fender American Vintage 1962 Jaguar, Paint"));
			charGuitar.Items.Add(new RBString("jaguar01_sparkle", "Fender American Vintage 1962 Jaguar, Sparkle"));
			charGuitar.Items.Add(new RBString("jaguar01_sunburstblack", "Fender American Vintage 1962 Jaguar, Black Sunburst"));
			charGuitar.Items.Add(new RBString("jaguar01_sunburstpearl", "Fender American Vintage 1962 Jaguar, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("jaguar01_sunbursttortoise", "Fender American Vintage 1962 Jaguar, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("jaguar01_sunburstwhite", "Fender American Vintage 1962 Jaguar, White Sunburst"));
			charGuitar.Items.Add(new RBString("jaguar01_triburstblack", "Fender American Vintage 1962 Jaguar, Black Triburst"));
			charGuitar.Items.Add(new RBString("jaguar01_triburstpearl", "Fender American Vintage 1962 Jaguar, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("jaguar01_tribursttortoise", "Fender American Vintage 1962 Jaguar, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("jaguar01_triburstwhite", "Fender American Vintage 1962 Jaguar, White Triburst"));
			charGuitar.Items.Add(new RBString("jaguar01_woodash", "Fender American Vintage 1962 Jaguar, Ash"));
			charGuitar.Items.Add(new RBString("jaguar01_woodmaple", "Fender American Vintage 1962 Jaguar, Maple"));
			charGuitar.Items.Add(new RBString("jaguar02_paint", "Fender Jaguar Baritone Special HH, Paint"));
			charGuitar.Items.Add(new RBString("jaguar02_sparkle", "Fender Jaguar Baritone Special HH, Sparkle"));
			charGuitar.Items.Add(new RBString("jaguar02_sunburstblack", "Fender Jaguar Baritone Special HH, Black Sunburst"));
			charGuitar.Items.Add(new RBString("jaguar02_sunburstpearl", "Fender Jaguar Baritone Special HH, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("jaguar02_sunbursttortoise", "Fender Jaguar Baritone Special HH, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("jaguar02_sunburstwhite", "Fender Jaguar Baritone Special HH, White Sunburst"));
			charGuitar.Items.Add(new RBString("jaguar02_triburstblack", "Fender Jaguar Baritone Special HH, Black Triburst"));
			charGuitar.Items.Add(new RBString("jaguar02_triburstpearl", "Fender Jaguar Baritone Special HH, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("jaguar02_tribursttortoise", "Fender Jaguar Baritone Special HH, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("jaguar02_triburstwhite", "Fender Jaguar Baritone Special HH, White Triburst"));
			charGuitar.Items.Add(new RBString("jaguar02_woodash", "Fender Jaguar Baritone Special HH, Ash"));
			charGuitar.Items.Add(new RBString("jaguar02_woodmaple", "Fender Jaguar Baritone Special HH, Maple"));
			charGuitar.Items.Add(new RBString("jaguar03_paint", "Fender 1966 Jaguar, Paint"));
			charGuitar.Items.Add(new RBString("jaguar03_sparkle", "Fender 1966 Jaguar, Sparkle"));
			charGuitar.Items.Add(new RBString("jaguar03_sunburstblack", "Fender 1966 Jaguar, Black Sunburst"));
			charGuitar.Items.Add(new RBString("jaguar03_sunburstpearl", "Fender 1966 Jaguar, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("jaguar03_sunbursttortoise", "Fender 1966 Jaguar, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("jaguar03_sunburstwhite", "Fender 1966 Jaguar, White Sunburst"));
			charGuitar.Items.Add(new RBString("jaguar03_triburstblack", "Fender 1966 Jaguar, Black Triburst"));
			charGuitar.Items.Add(new RBString("jaguar03_triburstpearl", "Fender 1966 Jaguar, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("jaguar03_tribursttortoise", "Fender 1966 Jaguar, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("jaguar03_triburstwhite", "Fender 1966 Jaguar, White Triburst"));
			charGuitar.Items.Add(new RBString("jaguar03_woodash", "Fender 1966 Jaguar, Ash"));
			charGuitar.Items.Add(new RBString("jaguar03_woodmaple", "Fender 1966 Jaguar, Maple"));
			charGuitar.Items.Add(new RBString("jetdoubleneck_paint", "Gretsch G5566 Electromatic Jet Double Neck"));
			charGuitar.Items.Add(new RBString("jp80_paint", "Fender JP80, Paint"));
			charGuitar.Items.Add(new RBString("jp80_sparkle", "Fender JP80, Sparkle"));
			charGuitar.Items.Add(new RBString("jp80_sunburstblack", "Fender JP80, Black Sunburst"));
			charGuitar.Items.Add(new RBString("jp80_sunburstpearl", "Fender JP80, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("jp80_sunbursttortoise", "Fender JP80, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("jp80_sunburstwhite", "Fender JP80, White Sunburst"));
			charGuitar.Items.Add(new RBString("jp80_triburstblack", "Fender JP80, Black Triburst"));
			charGuitar.Items.Add(new RBString("jp80_triburstpearl", "Fender JP80, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("jp80_tribursttortoise", "Fender JP80, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("jp80_triburstwhite", "Fender JP80, White Triburst"));
			charGuitar.Items.Add(new RBString("jp80_woodash", "Fender JP80, Ash"));
			charGuitar.Items.Add(new RBString("jp80_woodmaple", "Fender JP80, Maple"));
			charGuitar.Items.Add(new RBString("jupiter_resource", "Gretsch G6199 'Billy-Bo' Jupiter Thunderbird"));
			charGuitar.Items.Add(new RBString("kelly01_paint", "Jackson JS30KE Kelly, Paint"));
			charGuitar.Items.Add(new RBString("kelly01_sparkle", "Jackson JS30KE Kelly, Sparkle"));
			charGuitar.Items.Add(new RBString("kelly01_sunburst", "Jackson JS30KE Kelly, Black Sunburst"));
			charGuitar.Items.Add(new RBString("kelly01_triburst", "Jackson JS30KE Kelly, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("kelly01_woodash", "Jackson JS30KE Kelly, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("kelly01_woodmaple", "Jackson JS30KE Kelly, White Sunburst"));
			charGuitar.Items.Add(new RBString("kelly02_paint", "Jackson Custom Kelly, Black Triburst"));
			charGuitar.Items.Add(new RBString("kelly02_sparkle", "Jackson Custom Kelly, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("kelly02_sunburst", "Jackson Custom Kelly, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("kelly02_triburst", "Jackson Custom Kelly, White Triburst"));
			charGuitar.Items.Add(new RBString("kelly02_woodash", "Jackson Custom Kelly, Ash"));
			charGuitar.Items.Add(new RBString("kelly02_woodmaple", "Jackson Custom Kelly, Maple"));
			charGuitar.Items.Add(new RBString("kelly03_paint", "Jackson Custom Kelly Reversed, Paint"));
			charGuitar.Items.Add(new RBString("kelly03_sparkle", "Jackson Custom Kelly Reversed, Sparkle"));
			charGuitar.Items.Add(new RBString("kelly03_sunburst", "Jackson Custom Kelly Reversed, Sunburst"));
			charGuitar.Items.Add(new RBString("kelly03_triburst", "Jackson Custom Kelly Reversed, Triburst"));
			charGuitar.Items.Add(new RBString("kelly03_woodash", "Jackson Custom Kelly Reversed, Ash"));
			charGuitar.Items.Add(new RBString("kelly03_woodmaple", "Jackson Custom Kelly Reversed, Maple"));
			charGuitar.Items.Add(new RBString("kingv01_paint", "Jackson JS30KV King V, Paint"));
			charGuitar.Items.Add(new RBString("kingv01_sparkle", "Jackson JS30KV King V, Sparkle"));
			charGuitar.Items.Add(new RBString("kingv01_sunburst", "Jackson JS30KV King V, Sunburst"));
			charGuitar.Items.Add(new RBString("kingv01_triburst", "Jackson JS30KV King V, Triburst"));
			charGuitar.Items.Add(new RBString("kingv01_woodash", "Jackson JS30KV King V, Ash"));
			charGuitar.Items.Add(new RBString("kingv01_woodmaple", "Jackson JS30KV King V, Maple"));
			charGuitar.Items.Add(new RBString("kingv02_paint", "Jackson KVX10 King V, Paint"));
			charGuitar.Items.Add(new RBString("kingv02_sparkle", "Jackson KVX10 King V, Sparkle"));
			charGuitar.Items.Add(new RBString("kingv02_sunburst", "Jackson KVX10 King V, Sunburst"));
			charGuitar.Items.Add(new RBString("kingv02_triburst", "Jackson KVX10 King V, Triburst"));
			charGuitar.Items.Add(new RBString("kingv02_woodash", "Jackson KVX10 King V, Ash"));
			charGuitar.Items.Add(new RBString("kingv02_woodmaple", "Jackson KVX10 King V, Maple"));
			charGuitar.Items.Add(new RBString("kingv03_paint", "Jackson KV2T King V, Paint"));
			charGuitar.Items.Add(new RBString("kingv03_sparkle", "Jackson KV2T King V, Sparkle"));
			charGuitar.Items.Add(new RBString("kingv03_sunburst", "Jackson KV2T King V, Sunburst"));
			charGuitar.Items.Add(new RBString("kingv03_triburst", "Jackson KV2T King V, Triburst"));
			charGuitar.Items.Add(new RBString("kingv03_woodash", "Jackson KV2T King V, Ash"));
			charGuitar.Items.Add(new RBString("kingv03_woodmaple", "Jackson KV2T King V, Maple"));
			charGuitar.Items.Add(new RBString("kingv04_paint", "Jackson Custom Double Neck King V, Paint"));
			charGuitar.Items.Add(new RBString("kingv04_sparkle", "Jackson Custom Double Neck King V, Sparkle"));
			charGuitar.Items.Add(new RBString("kingv04_sunburst", "Jackson Custom Double Neck King V, Sunburst"));
			charGuitar.Items.Add(new RBString("kingv04_triburst", "Jackson Custom Double Neck King V, Triburst"));
			charGuitar.Items.Add(new RBString("kingv04_woodash", "Jackson Custom Double Neck King V, Ash"));
			charGuitar.Items.Add(new RBString("kingv04_woodmaple", "Jackson Custom Double Neck King V, Maple"));
			charGuitar.Items.Add(new RBString("korina_paint", "Hamer Special Korina Jr, Paint"));
			charGuitar.Items.Add(new RBString("korina_sparkle", "Hamer Special Korina Jr, Sparkle"));
			charGuitar.Items.Add(new RBString("korina_sunburstblack", "Hamer Special Korina Jr, Black Sunburst"));
			charGuitar.Items.Add(new RBString("korina_sunburstpearl", "Hamer Special Korina Jr, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("korina_sunbursttortoise", "Hamer Special Korina Jr, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("korina_sunburstwhite", "Hamer Special Korina Jr, White Sunburst"));
			charGuitar.Items.Add(new RBString("korina_triburstblack", "Hamer Special Korina Jr, Black Triburst"));
			charGuitar.Items.Add(new RBString("korina_triburstpearl", "Hamer Special Korina Jr, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("korina_tribursttortoise", "Hamer Special Korina Jr, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("korina_triburstwhite", "Hamer Special Korina Jr, White Triburst"));
			charGuitar.Items.Add(new RBString("korina_woodash", "Hamer Special Korina Jr, Ash"));
			charGuitar.Items.Add(new RBString("korina_woodmaple", "Hamer Special Korina Jr, Maple"));
			charGuitar.Items.Add(new RBString("m1969_paint", "Fender M1969, Paint"));
			charGuitar.Items.Add(new RBString("m1969_sparkle", "Fender M1969, Sparkle"));
			charGuitar.Items.Add(new RBString("m1969_sunburstblack", "Fender M1969, Black Sunburst"));
			charGuitar.Items.Add(new RBString("m1969_sunburstpearl", "Fender M1969, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("m1969_sunbursttortoise", "Fender M1969, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("m1969_sunburstwhite", "Fender M1969, White Sunburst"));
			charGuitar.Items.Add(new RBString("m1969_triburstblack", "Fender M1969, Black Triburst"));
			charGuitar.Items.Add(new RBString("m1969_triburstpearl", "Fender M1969, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("m1969_tribursttortoise", "Fender M1969, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("m1969_triburstwhite", "Fender M1969, White Triburst"));
			charGuitar.Items.Add(new RBString("m1969_woodash", "Fender M1969, Ash"));
			charGuitar.Items.Add(new RBString("m1969_woodmaple", "Fender M1969, Maple"));
			charGuitar.Items.Add(new RBString("mace_resource", "The Mace"));
			charGuitar.Items.Add(new RBString("malcolm_paint", "Gretsch Malcolm Young I, Paint"));
			charGuitar.Items.Add(new RBString("malcolm_sparkle", "Gretsch Malcolm Young I, Sparkle"));
			charGuitar.Items.Add(new RBString("malcolm_sunburst", "Gretsch Malcolm Young I, Sunburst"));
			charGuitar.Items.Add(new RBString("malcolm_triburst", "Gretsch Malcolm Young I, Triburst"));
			charGuitar.Items.Add(new RBString("malcolm_woodash", "Gretsch Malcolm Young I, Ash"));
			charGuitar.Items.Add(new RBString("malcolm_woodmaple", "Gretsch Malcolm Young I, Maple"));
			charGuitar.Items.Add(new RBString("mustang02_paint", "Fender '65 Mustang, Paint"));
			charGuitar.Items.Add(new RBString("mustang02_sparkle", "Fender '65 Mustang, Sparkle"));
			charGuitar.Items.Add(new RBString("mustang02_sunburstblack", "Fender '65 Mustang, Black Sunburst"));
			charGuitar.Items.Add(new RBString("mustang02_sunburstpearl", "Fender '65 Mustang, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("mustang02_sunbursttortoise", "Fender '65 Mustang, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("mustang02_sunburstwhite", "Fender '65 Mustang, White Sunburst"));
			charGuitar.Items.Add(new RBString("mustang02_triburstblack", "Fender '65 Mustang, Black Triburst"));
			charGuitar.Items.Add(new RBString("mustang02_triburstpearl", "Fender '65 Mustang, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("mustang02_tribursttortoise", "Fender '65 Mustang, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("mustang02_triburstwhite", "Fender '65 Mustang, White Triburst"));
			charGuitar.Items.Add(new RBString("mustang02_woodash", "Fender '65 Mustang, Ash"));
			charGuitar.Items.Add(new RBString("mustang02_woodmaple", "Fender '65 Mustang, Maple"));
			charGuitar.Items.Add(new RBString("nashdoubleneck_paint", "Gretsch G6120-6/12 Nashville Double Neck"));
			charGuitar.Items.Add(new RBString("neon_resource", "Neon Dream"));
			charGuitar.Items.Add(new RBString("newport_paint", "Hamer Newport Pro Custom, Paint"));
			charGuitar.Items.Add(new RBString("newport_sparkle", "Hamer Newport Pro Custom, Sparkle"));
			charGuitar.Items.Add(new RBString("newport_sunburst", "Hamer Newport Pro Custom, Sunburst"));
			charGuitar.Items.Add(new RBString("newport_triburst", "Hamer Newport Pro Custom, Triburst"));
			charGuitar.Items.Add(new RBString("newport_woodash", "Hamer Newport Pro Custom, Ash"));
			charGuitar.Items.Add(new RBString("newport_woodmaple", "Hamer Newport Pro Custom, Maple"));
			charGuitar.Items.Add(new RBString("p2010_paint", "Ovation P2010, Paint"));
			charGuitar.Items.Add(new RBString("p2010_sparkle", "Ovation P2010, Sparkle"));
			charGuitar.Items.Add(new RBString("p2010_sunburstblack", "Ovation P2010, Black Sunburst"));
			charGuitar.Items.Add(new RBString("p2010_sunburstpearl", "Ovation P2010, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("p2010_sunbursttortoise", "Ovation P2010, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("p2010_sunburstwhite", "Ovation P2010, White Sunburst"));
			charGuitar.Items.Add(new RBString("p2010_triburstblack", "Ovation P2010, Black Triburst"));
			charGuitar.Items.Add(new RBString("p2010_triburstpearl", "Ovation P2010, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("p2010_tribursttortoise", "Ovation P2010, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("p2010_triburstwhite", "Ovation P2010, White Triburst"));
			charGuitar.Items.Add(new RBString("p2010_woodash", "Ovation P2010, Ash"));
			charGuitar.Items.Add(new RBString("p2010_woodmaple", "Ovation P2010, Maple"));
			charGuitar.Items.Add(new RBString("penguin_paint", "Gretsch G6134 White Penguin"));
			charGuitar.Items.Add(new RBString("polara_paint", "Guild Polara, Paint"));
			charGuitar.Items.Add(new RBString("polara_sparkle", "Guild Polara, Sparkle"));
			charGuitar.Items.Add(new RBString("polara_sunburstblack", "Guild Polara, Black Sunburst"));
			charGuitar.Items.Add(new RBString("polara_sunburstpearl", "Guild Polara, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("polara_sunbursttortoise", "Guild Polara, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("polara_sunburstwhite", "Guild Polara, White Sunburst"));
			charGuitar.Items.Add(new RBString("polara_triburstblack", "Guild Polara, Black Triburst"));
			charGuitar.Items.Add(new RBString("polara_triburstpearl", "Guild Polara, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("polara_tribursttortoise", "Guild Polara, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("polara_triburstwhite", "Guild Polara, White Triburst"));
			charGuitar.Items.Add(new RBString("polara_woodash", "Guild Polara, Ash"));
			charGuitar.Items.Add(new RBString("polara_woodmaple", "Guild Polara, Maple"));
			charGuitar.Items.Add(new RBString("prefish_resource", "Bubbles"));
			charGuitar.Items.Add(new RBString("projet_paint", "Gretsch G5235T Electromatic Pro Jet, Paint"));
			charGuitar.Items.Add(new RBString("projet_sparkle", "Gretsch G5235T Electromatic Pro Jet, Sparkle"));
			charGuitar.Items.Add(new RBString("projet_sunburstblack", "Gretsch G5235T Electromatic Pro Jet, Black Sunburst"));
			charGuitar.Items.Add(new RBString("projet_sunburstpearl", "Gretsch G5235T Electromatic Pro Jet, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("projet_sunbursttortoise", "Gretsch G5235T Electromatic Pro Jet, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("projet_sunburstwhite", "Gretsch G5235T Electromatic Pro Jet, White Sunburst"));
			charGuitar.Items.Add(new RBString("projet_triburstblack", "Gretsch G5235T Electromatic Pro Jet, Black Triburst"));
			charGuitar.Items.Add(new RBString("projet_triburstpearl", "Gretsch G5235T Electromatic Pro Jet, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("projet_tribursttortoise", "Gretsch G5235T Electromatic Pro Jet, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("projet_triburstwhite", "Gretsch G5235T Electromatic Pro Jet, White Triburst"));
			charGuitar.Items.Add(new RBString("projet_woodash", "Gretsch G5235T Electromatic Pro Jet, Ash"));
			charGuitar.Items.Add(new RBString("projet_woodmaple", "Gretsch G5235T Electromatic Pro Jet, Maple"));
			charGuitar.Items.Add(new RBString("s100_paint", "Guild S-100, Paint"));
			charGuitar.Items.Add(new RBString("s100_sparkle", "Guild S-100, Sparkle"));
			charGuitar.Items.Add(new RBString("s100_sunburstblack", "Guild S-100, Black Sunburst"));
			charGuitar.Items.Add(new RBString("s100_sunburstpearl", "Guild S-100, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("s100_sunbursttortoise", "Guild S-100, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("s100_sunburstwhite", "Guild S-100, White Sunburst"));
			charGuitar.Items.Add(new RBString("s100_triburstblack", "Guild S-100, Black Triburst"));
			charGuitar.Items.Add(new RBString("s100_triburstpearl", "Guild S-100, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("s100_tribursttortoise", "Guild S-100, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("s100_triburstwhite", "Guild S-100, White Triburst"));
			charGuitar.Items.Add(new RBString("s100_woodash", "Guild S-100, Ash"));
			charGuitar.Items.Add(new RBString("s100_woodmaple", "Guild S-100, Maple"));
			charGuitar.Items.Add(new RBString("s300_paint", "Guild S-300, Paint"));
			charGuitar.Items.Add(new RBString("s300_sparkle", "Guild S-300, Sparkle"));
			charGuitar.Items.Add(new RBString("s300_sunburstblack", "Guild S-300, Black Sunburst"));
			charGuitar.Items.Add(new RBString("s300_sunburstpearl", "Guild S-300, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("s300_sunbursttortoise", "Guild S-300, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("s300_sunburstwhite", "Guild S-300, White Sunburst"));
			charGuitar.Items.Add(new RBString("s300_triburstblack", "Guild S-300, Black Triburst"));
			charGuitar.Items.Add(new RBString("s300_triburstpearl", "Guild S-300, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("s300_tribursttortoise", "Guild S-300, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("s300_triburstwhite", "Guild S-300, White Triburst"));
			charGuitar.Items.Add(new RBString("s300_woodash", "Guild S-300, Ash"));
			charGuitar.Items.Add(new RBString("s300_woodmaple", "Guild S-300, Maple"));
			charGuitar.Items.Add(new RBString("skeletar_resource", "SkeleTone"));
			charGuitar.Items.Add(new RBString("skull_resource", "The Skull"));
			charGuitar.Items.Add(new RBString("starcaster_paint", "Fender Starcaster, Paint"));
			charGuitar.Items.Add(new RBString("starcaster_sparkle", "Fender Starcaster, Sparkle"));
			charGuitar.Items.Add(new RBString("starcaster_sunburstblack", "Fender Starcaster, Black Sunburst"));
			charGuitar.Items.Add(new RBString("starcaster_sunburstpearl", "Fender Starcaster, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("starcaster_sunbursttortoise", "Fender Starcaster, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("starcaster_sunburstwhite", "Fender Starcaster, White Sunburst"));
			charGuitar.Items.Add(new RBString("starcaster_triburstblack", "Fender Starcaster, Black Triburst"));
			charGuitar.Items.Add(new RBString("starcaster_triburstpearl", "Fender Starcaster, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("starcaster_tribursttortoise", "Fender Starcaster, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("starcaster_triburstwhite", "Fender Starcaster, White Triburst"));
			charGuitar.Items.Add(new RBString("starcaster_woodash", "Fender Starcaster, Ash"));
			charGuitar.Items.Add(new RBString("starcaster_woodmaple", "Fender Starcaster, Maple"));
			charGuitar.Items.Add(new RBString("stratocaster01_paint", "Fender Standard Stratocaster, Paint"));
			charGuitar.Items.Add(new RBString("stratocaster01_sparkle", "Fender Standard Stratocaster, Sparkle"));
			charGuitar.Items.Add(new RBString("stratocaster01_sunburstblack", "Fender Standard Stratocaster, Black Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster01_sunburstpearl", "Fender Standard Stratocaster, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster01_sunbursttortoise", "Fender Standard Stratocaster, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster01_sunburstwhite", "Fender Standard Stratocaster, White Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster01_triburstblack", "Fender Standard Stratocaster, Black Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster01_triburstpearl", "Fender Standard Stratocaster, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster01_tribursttortoise", "Fender Standard Stratocaster, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster01_triburstwhite", "Fender Standard Stratocaster, White Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster01_woodash", "Fender Standard Stratocaster, Ash"));
			charGuitar.Items.Add(new RBString("stratocaster01_woodmaple", "Fender Standard Stratocaster, Maple"));
			charGuitar.Items.Add(new RBString("stratocaster02_paint", "Fender Big Block Stratocaster, Paint"));
			charGuitar.Items.Add(new RBString("stratocaster02_sparkle", "Fender Big Block Stratocaster, Sparkle"));
			charGuitar.Items.Add(new RBString("stratocaster02_sunburstblack", "Fender Big Block Stratocaster, Black Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster02_sunburstpearl", "Fender Big Block Stratocaster, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster02_sunbursttortoise", "Fender Big Block Stratocaster, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster02_sunburstwhite", "Fender Big Block Stratocaster, White Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster02_triburstblack", "Fender Big Block Stratocaster, Black Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster02_triburstpearl", "Fender Big Block Stratocaster, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster02_tribursttortoise", "Fender Big Block Stratocaster, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster02_triburstwhite", "Fender Big Block Stratocaster, White Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster02_woodash", "Fender Big Block Stratocaster, Ash"));
			charGuitar.Items.Add(new RBString("stratocaster02_woodmaple", "Fender Big Block Stratocaster, Maple"));
			charGuitar.Items.Add(new RBString("stratocaster03clear_default", "Clear Fender"));
			charGuitar.Items.Add(new RBString("stratocaster03gold_default", "Gold Fender"));
			charGuitar.Items.Add(new RBString("stratocaster03silver_default", "Silver Fender"));
			charGuitar.Items.Add(new RBString("stratocaster03_paint", "Fender American Deluxe Stratocaster, Paint"));
			charGuitar.Items.Add(new RBString("stratocaster03_sparkle", "Fender American Deluxe Stratocaster, Sparkle"));
			charGuitar.Items.Add(new RBString("stratocaster03_sunburst", "Fender American Deluxe Stratocaster, Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster03_triburst", "Fender American Deluxe Stratocaster, Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster03_woodash", "Fender American Deluxe Stratocaster, Ash"));
			charGuitar.Items.Add(new RBString("stratocaster03_woodmaple", "Fender American Deluxe Stratocaster, Maple"));
			charGuitar.Items.Add(new RBString("stratocaster04_paint", "Fender Stratocaster 12-String, Paint"));
			charGuitar.Items.Add(new RBString("stratocaster04_sparkle", "Fender Stratocaster 12-String, Sparkle"));
			charGuitar.Items.Add(new RBString("stratocaster04_sunburstblack", "Fender Stratocaster 12-String, Black Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster04_sunburstpearl", "Fender Stratocaster 12-String, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster04_sunbursttortoise", "Fender Stratocaster 12-String, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster04_sunburstwhite", "Fender Stratocaster 12-String, White Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster04_triburstblack", "Fender Stratocaster 12-String, Black Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster04_triburstpearl", "Fender Stratocaster 12-String, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster04_tribursttortoise", "Fender Stratocaster 12-String, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster04_triburstwhite", "Fender Stratocaster 12-String, White Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster04_woodash", "Fender Stratocaster 12-String, Ash"));
			charGuitar.Items.Add(new RBString("stratocaster04_woodmaple", "Fender Stratocaster 12-String, Maple"));
			charGuitar.Items.Add(new RBString("stratocaster05_paint", "Fender Custom Double Neck Stratocaster, Paint"));
			charGuitar.Items.Add(new RBString("stratocaster05_sparkle", "Fender Custom Double Neck Stratocaster, Sparkle"));
			charGuitar.Items.Add(new RBString("stratocaster05_sunburstblack", "Fender Custom Double Neck Stratocaster, Black Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster05_sunburstpearl", "Fender Custom Double Neck Stratocaster, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster05_sunbursttortoise", "Fender Custom Double Neck Stratocaster, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster05_sunburstwhite", "Fender Custom Double Neck Stratocaster, White Sunburst"));
			charGuitar.Items.Add(new RBString("stratocaster05_triburstblack", "Fender Custom Double Neck Stratocaster, Black Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster05_triburstpearl", "Fender Custom Double Neck Stratocaster, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster05_tribursttortoise", "Fender Custom Double Neck Stratocaster, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster05_triburstwhite", "Fender Custom Double Neck Stratocaster, White Triburst"));
			charGuitar.Items.Add(new RBString("stratocaster05_woodash", "Fender Custom Double Neck Stratocaster, Ash"));
			charGuitar.Items.Add(new RBString("stratocaster05_woodmaple", "Fender Custom Double Neck Stratocaster, Maple"));
			charGuitar.Items.Add(new RBString("supersonic_paint", "Squier Super-Sonic, Paint"));
			charGuitar.Items.Add(new RBString("supersonic_sparkle", "Squier Super-Sonic, Sparkle"));
			charGuitar.Items.Add(new RBString("supersonic_sunburstblack", "Squier Super-Sonic, Black Sunburst"));
			charGuitar.Items.Add(new RBString("supersonic_sunburstpearl", "Squier Super-Sonic, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("supersonic_sunbursttortoise", "Squier Super-Sonic, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("supersonic_sunburstwhite", "Squier Super-Sonic, White Sunburst"));
			charGuitar.Items.Add(new RBString("supersonic_triburstblack", "Squier Super-Sonic, Black Triburst"));
			charGuitar.Items.Add(new RBString("supersonic_triburstpearl", "Squier Super-Sonic, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("supersonic_tribursttortoise", "Squier Super-Sonic, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("supersonic_triburstwhite", "Squier Super-Sonic, White Triburst"));
			charGuitar.Items.Add(new RBString("supersonic_woodash", "Squier Super-Sonic, Ash"));
			charGuitar.Items.Add(new RBString("supersonic_woodmaple", "Squier Super-Sonic, Maple"));
			charGuitar.Items.Add(new RBString("swinger_paint", "Fender Swinger, Paint"));
			charGuitar.Items.Add(new RBString("swinger_sparkle", "Fender Swinger, Sparkle"));
			charGuitar.Items.Add(new RBString("swinger_sunburstblack", "Fender Swinger, Black Sunburst"));
			charGuitar.Items.Add(new RBString("swinger_sunburstpearl", "Fender Swinger, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("swinger_sunbursttortoise", "Fender Swinger, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("swinger_sunburstwhite", "Fender Swinger, White Sunburst"));
			charGuitar.Items.Add(new RBString("swinger_triburstblack", "Fender Swinger, Black Triburst"));
			charGuitar.Items.Add(new RBString("swinger_triburstpearl", "Fender Swinger, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("swinger_tribursttortoise", "Fender Swinger, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("swinger_triburstwhite", "Fender Swinger, White Triburst"));
			charGuitar.Items.Add(new RBString("swinger_woodash", "Fender Swinger, Ash"));
			charGuitar.Items.Add(new RBString("swinger_woodmaple", "Fender Swinger, Maple"));
			charGuitar.Items.Add(new RBString("synchromatic_paint", "Gretsch G6040MCSS Synchromatic Cutaway Filter'Tron, Paint"));
			charGuitar.Items.Add(new RBString("synchromatic_sparkle", "Gretsch G6040MCSS Synchromatic Cutaway Filter'Tron, Sparkle"));
			charGuitar.Items.Add(new RBString("synchromatic_sunburstblack", "Gretsch G6040MCSS Synchromatic Cutaway Filter'Tron, Black Sunburst"));
			charGuitar.Items.Add(new RBString("synchromatic_sunburstpearl", "Gretsch G6040MCSS Synchromatic Cutaway Filter'Tron, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("synchromatic_sunbursttortoise", "Gretsch G6040MCSS Synchromatic Cutaway Filter'Tron, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("synchromatic_sunburstwhite", "Gretsch G6040MCSS Synchromatic Cutaway Filter'Tron, White Sunburst"));
			charGuitar.Items.Add(new RBString("synchromatic_triburstblack", "Gretsch G6040MCSS Synchromatic Cutaway Filter'Tron, Black Triburst"));
			charGuitar.Items.Add(new RBString("synchromatic_triburstpearl", "Gretsch G6040MCSS Synchromatic Cutaway Filter'Tron, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("synchromatic_tribursttortoise", "Gretsch G6040MCSS Synchromatic Cutaway Filter'Tron, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("synchromatic_triburstwhite", "Gretsch G6040MCSS Synchromatic Cutaway Filter'Tron, White Triburst"));
			charGuitar.Items.Add(new RBString("synchromatic_woodash", "Gretsch G6040MCSS Synchromatic Cutaway Filter'Tron, Ash"));
			charGuitar.Items.Add(new RBString("synchromatic_woodmaple", "Gretsch G6040MCSS Synchromatic Cutaway Filter'Tron, Maple"));
			charGuitar.Items.Add(new RBString("tc90_paint", "Fender TC-90 Thinline, Paint"));
			charGuitar.Items.Add(new RBString("tc90_sparkle", "Fender TC-90 Thinline, Sparkle"));
			charGuitar.Items.Add(new RBString("tc90_sunburstblack", "Fender TC-90 Thinline, Black Sunburst"));
			charGuitar.Items.Add(new RBString("tc90_sunburstpearl", "Fender TC-90 Thinline, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("tc90_sunbursttortoise", "Fender TC-90 Thinline, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("tc90_sunburstwhite", "Fender TC-90 Thinline, White Sunburst"));
			charGuitar.Items.Add(new RBString("tc90_triburstblack", "Fender TC-90 Thinline, Black Triburst"));
			charGuitar.Items.Add(new RBString("tc90_triburstpearl", "Fender TC-90 Thinline, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("tc90_tribursttortoise", "Fender TC-90 Thinline, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("tc90_triburstwhite", "Fender TC-90 Thinline, White Triburst"));
			charGuitar.Items.Add(new RBString("tc90_woodash", "Fender TC-90 Thinline, Ash"));
			charGuitar.Items.Add(new RBString("tc90_woodmaple", "Fender TC-90 Thinline, Maple"));
			charGuitar.Items.Add(new RBString("teardrop_resource", "The Teardrop"));
			charGuitar.Items.Add(new RBString("telecaster01_paint", "Fender Classic '50s Telecaster, Paint"));
			charGuitar.Items.Add(new RBString("telecaster01_sparkle", "Fender Classic '50s Telecaster, Sparkle"));
			charGuitar.Items.Add(new RBString("telecaster01_sunburstblack", "Fender Classic '50s Telecaster, Black Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster01_sunburstpearl", "Fender Classic '50s Telecaster, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster01_sunbursttortoise", "Fender Classic '50s Telecaster, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster01_sunburstwhite", "Fender Classic '50s Telecaster, White Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster01_triburstblack", "Fender Classic '50s Telecaster, Black Triburst"));
			charGuitar.Items.Add(new RBString("telecaster01_triburstpearl", "Fender Classic '50s Telecaster, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("telecaster01_tribursttortoise", "Fender Classic '50s Telecaster, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("telecaster01_triburstwhite", "Fender Classic '50s Telecaster, White Triburst"));
			charGuitar.Items.Add(new RBString("telecaster01_woodash", "Fender Classic '50s Telecaster, Ash"));
			charGuitar.Items.Add(new RBString("telecaster01_woodmaple", "Fender Classic '50s Telecaster, Maple"));
			charGuitar.Items.Add(new RBString("telecaster02_paint", "Fender 1969 Thinline Telecaster, Paint"));
			charGuitar.Items.Add(new RBString("telecaster02_sparkle", "Fender 1969 Thinline Telecaster, Sparkle"));
			charGuitar.Items.Add(new RBString("telecaster02_sunburstblack", "Fender 1969 Thinline Telecaster, Black Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster02_sunburstpearl", "Fender 1969 Thinline Telecaster, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster02_sunbursttortoise", "Fender 1969 Thinline Telecaster, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster02_sunburstwhite", "Fender 1969 Thinline Telecaster, White Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster02_triburstblack", "Fender 1969 Thinline Telecaster, Black Triburst"));
			charGuitar.Items.Add(new RBString("telecaster02_triburstpearl", "Fender 1969 Thinline Telecaster, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("telecaster02_tribursttortoise", "Fender 1969 Thinline Telecaster, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("telecaster02_triburstwhite", "Fender 1969 Thinline Telecaster, White Triburst"));
			charGuitar.Items.Add(new RBString("telecaster02_woodash", "Fender 1969 Thinline Telecaster, Ash"));
			charGuitar.Items.Add(new RBString("telecaster02_woodmaple", "Fender 1969 Thinline Telecaster, Maple"));
			charGuitar.Items.Add(new RBString("telecaster03_paint", "Fender 1972 Telecaster, Paint"));
			charGuitar.Items.Add(new RBString("telecaster03_sparkle", "Fender 1972 Telecaster, Sparkle"));
			charGuitar.Items.Add(new RBString("telecaster03_sunburstblack", "Fender 1972 Telecaster, Black Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster03_sunburstpearl", "Fender 1972 Telecaster, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster03_sunbursttortoise", "Fender 1972 Telecaster, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster03_sunburstwhite", "Fender 1972 Telecaster, White Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster03_triburstblack", "Fender 1972 Telecaster, Black Triburst"));
			charGuitar.Items.Add(new RBString("telecaster03_triburstpearl", "Fender 1972 Telecaster, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("telecaster03_tribursttortoise", "Fender 1972 Telecaster, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("telecaster03_triburstwhite", "Fender 1972 Telecaster, White Triburst"));
			charGuitar.Items.Add(new RBString("telecaster03_woodash", "Fender 1972 Telecaster, Ash"));
			charGuitar.Items.Add(new RBString("telecaster03_woodmaple", "Fender 1972 Telecaster, Maple"));
			charGuitar.Items.Add(new RBString("telecaster04_paint", "Fender Custom Telecaster FMT HH, Paint"));
			charGuitar.Items.Add(new RBString("telecaster04_sparkle", "Fender Custom Telecaster FMT HH, Sparkle"));
			charGuitar.Items.Add(new RBString("telecaster04_sunburst", "Fender Custom Telecaster FMT HH, Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster04_triburst", "Fender Custom Telecaster FMT HH, Triburst"));
			charGuitar.Items.Add(new RBString("telecaster04_woodash", "Fender Custom Telecaster FMT HH, Ash"));
			charGuitar.Items.Add(new RBString("telecaster04_woodmaple", "Fender Custom Telecaster FMT HH, Maple"));
			charGuitar.Items.Add(new RBString("telecaster05_paint", "Fender J5 Triple Telecaster Deluxe, Paint"));
			charGuitar.Items.Add(new RBString("telecaster05_sparkle", "Fender J5 Triple Telecaster Deluxe, Sparkle"));
			charGuitar.Items.Add(new RBString("telecaster05_sunburstblack", "Fender J5 Triple Telecaster Deluxe, Black Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster05_sunburstpearl", "Fender J5 Triple Telecaster Deluxe, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster05_sunbursttortoise", "Fender J5 Triple Telecaster Deluxe, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster05_sunburstwhite", "Fender J5 Triple Telecaster Deluxe, White Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster05_triburstblack", "Fender J5 Triple Telecaster Deluxe, Black Triburst"));
			charGuitar.Items.Add(new RBString("telecaster05_triburstpearl", "Fender J5 Triple Telecaster Deluxe, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("telecaster05_tribursttortoise", "Fender J5 Triple Telecaster Deluxe, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("telecaster05_triburstwhite", "Fender J5 Triple Telecaster Deluxe, White Triburst"));
			charGuitar.Items.Add(new RBString("telecaster05_woodash", "Fender J5 Triple Telecaster Deluxe, Ash"));
			charGuitar.Items.Add(new RBString("telecaster05_woodmaple", "Fender J5 Triple Telecaster Deluxe, Maple"));
			charGuitar.Items.Add(new RBString("telecaster06_paint", "Fender 1959 Custom Telecaster, Paint"));
			charGuitar.Items.Add(new RBString("telecaster06_sparkle", "Fender 1959 Custom Telecaster, Sparkle"));
			charGuitar.Items.Add(new RBString("telecaster06_sunburstblack", "Fender 1959 Custom Telecaster, Black Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster06_sunburstpearl", "Fender 1959 Custom Telecaster, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster06_sunbursttortoise", "Fender 1959 Custom Telecaster, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster06_sunburstwhite", "Fender 1959 Custom Telecaster, White Sunburst"));
			charGuitar.Items.Add(new RBString("telecaster06_triburstblack", "Fender 1959 Custom Telecaster, Black Triburst"));
			charGuitar.Items.Add(new RBString("telecaster06_triburstpearl", "Fender 1959 Custom Telecaster, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("telecaster06_tribursttortoise", "Fender 1959 Custom Telecaster, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("telecaster06_triburstwhite", "Fender 1959 Custom Telecaster, White Triburst"));
			charGuitar.Items.Add(new RBString("telecaster06_woodash", "Fender 1959 Custom Telecaster, Ash"));
			charGuitar.Items.Add(new RBString("telecaster06_woodmaple", "Fender 1959 Custom Telecaster, Maple"));
			charGuitar.Items.Add(new RBString("thehand_resource", "Hand of Gargalonn"));
			charGuitar.Items.Add(new RBString("toronado_paint", "Fender Toronado, Paint"));
			charGuitar.Items.Add(new RBString("toronado_sparkle", "Fender Toronado, Sparkle"));
			charGuitar.Items.Add(new RBString("toronado_sunburstblack", "Fender Toronado, Black Sunburst"));
			charGuitar.Items.Add(new RBString("toronado_sunburstpearl", "Fender Toronado, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("toronado_sunbursttortoise", "Fender Toronado, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("toronado_sunburstwhite", "Fender Toronado, White Sunburst"));
			charGuitar.Items.Add(new RBString("toronado_triburstblack", "Fender Toronado, Black Triburst"));
			charGuitar.Items.Add(new RBString("toronado_triburstpearl", "Fender Toronado, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("toronado_tribursttortoise", "Fender Toronado, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("toronado_triburstwhite", "Fender Toronado, White Triburst"));
			charGuitar.Items.Add(new RBString("toronado_woodash", "Fender Toronado, Ash"));
			charGuitar.Items.Add(new RBString("toronado_woodmaple", "Fender Toronado, Maple"));
			charGuitar.Items.Add(new RBString("v2010_paint", "Ovation V2010, Paint"));
			charGuitar.Items.Add(new RBString("v2010_sparkle", "Ovation V2010, Sparkle"));
			charGuitar.Items.Add(new RBString("v2010_sunburstblack", "Ovation V2010, Black Sunburst"));
			charGuitar.Items.Add(new RBString("v2010_sunburstpearl", "Ovation V2010, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("v2010_sunbursttortoise", "Ovation V2010, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("v2010_sunburstwhite", "Ovation V2010, White Sunburst"));
			charGuitar.Items.Add(new RBString("v2010_triburstblack", "Ovation V2010, Black Triburst"));
			charGuitar.Items.Add(new RBString("v2010_triburstpearl", "Ovation V2010, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("v2010_tribursttortoise", "Ovation V2010, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("v2010_triburstwhite", "Ovation V2010, White Triburst"));
			charGuitar.Items.Add(new RBString("v2010_woodash", "Ovation V2010, Ash"));
			charGuitar.Items.Add(new RBString("v2010_woodmaple", "Ovation V2010, Maple"));
			charGuitar.Items.Add(new RBString("warrior01_paint", "Jackson JS30WR Warrior, Paint"));
			charGuitar.Items.Add(new RBString("warrior01_sparkle", "Jackson JS30WR Warrior, Sparkle"));
			charGuitar.Items.Add(new RBString("warrior01_sunburst", "Jackson JS30WR Warrior, Sunburst"));
			charGuitar.Items.Add(new RBString("warrior01_triburst", "Jackson JS30WR Warrior, Triburst"));
			charGuitar.Items.Add(new RBString("warrior01_woodash", "Jackson JS30WR Warrior, Ash"));
			charGuitar.Items.Add(new RBString("warrior01_woodmaple", "Jackson JS30WR Warrior, Maple"));
			charGuitar.Items.Add(new RBString("warrior02_paint", "Jackson WRMG Warrior, Paint"));
			charGuitar.Items.Add(new RBString("warrior02_sparkle", "Jackson WRMG Warrior, Sparkle"));
			charGuitar.Items.Add(new RBString("warrior02_sunburst", "Jackson WRMG Warrior, Black Sunburst"));
			charGuitar.Items.Add(new RBString("warrior02_triburst", "Jackson WRMG Warrior, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("warrior02_woodash", "Jackson WRMG Warrior, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("warrior02_woodmaple", "Jackson WRMG Warrior, White Sunburst"));
			charGuitar.Items.Add(new RBString("warrior03_paint", "Jackson WRXT Warrior, Black Triburst"));
			charGuitar.Items.Add(new RBString("warrior03_sparkle", "Jackson WRXT Warrior, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("warrior03_sunburst", "Jackson WRXT Warrior, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("warrior03_triburst", "Jackson WRXT Warrior, White Triburst"));
			charGuitar.Items.Add(new RBString("warrior03_woodash", "Jackson WRXT Warrior, Ash"));
			charGuitar.Items.Add(new RBString("warrior03_woodmaple", "Jackson WRXT Warrior, Maple"));
			charGuitar.Items.Add(new RBString("wildwood_paint", "Fender Wildwood, Paint"));
			charGuitar.Items.Add(new RBString("wildwood_sparkle", "Fender Wildwood, Sparkle"));
			charGuitar.Items.Add(new RBString("wildwood_sunburstblack", "Fender Wildwood, Black Sunburst"));
			charGuitar.Items.Add(new RBString("wildwood_sunburstpearl", "Fender Wildwood, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("wildwood_sunbursttortoise", "Fender Wildwood, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("wildwood_sunburstwhite", "Fender Wildwood, White Sunburst"));
			charGuitar.Items.Add(new RBString("wildwood_triburstblack", "Fender Wildwood, Black Triburst"));
			charGuitar.Items.Add(new RBString("wildwood_triburstpearl", "Fender Wildwood, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("wildwood_tribursttortoise", "Fender Wildwood, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("wildwood_triburstwhite", "Fender Wildwood, White Triburst"));
			charGuitar.Items.Add(new RBString("wildwood_woodash", "Fender Wildwood, Ash"));
			charGuitar.Items.Add(new RBString("wildwood_woodmaple", "Fender Wildwood, Maple"));
			charGuitar.Items.Add(new RBString("wings_resource", "The Fallen Angel"));
			charGuitar.Items.Add(new RBString("x79_paint", "Guild X-79, Paint"));
			charGuitar.Items.Add(new RBString("x79_sparkle", "Guild X-79, Sparkle"));
			charGuitar.Items.Add(new RBString("x79_sunburstblack", "Guild X-79, Black Sunburst"));
			charGuitar.Items.Add(new RBString("x79_sunburstpearl", "Guild X-79, Pearl Sunburst"));
			charGuitar.Items.Add(new RBString("x79_sunbursttortoise", "Guild X-79, Tortoise Sunburst"));
			charGuitar.Items.Add(new RBString("x79_sunburstwhite", "Guild X-79, White Sunburst"));
			charGuitar.Items.Add(new RBString("x79_triburstblack", "Guild X-79, Black Triburst"));
			charGuitar.Items.Add(new RBString("x79_triburstpearl", "Guild X-79, Pearl Triburst"));
			charGuitar.Items.Add(new RBString("x79_tribursttortoise", "Guild X-79, Tortoise Triburst"));
			charGuitar.Items.Add(new RBString("x79_triburstwhite", "Guild X-79, White Triburst"));
			charGuitar.Items.Add(new RBString("x79_woodash", "Guild X-79, Ash"));
			charGuitar.Items.Add(new RBString("x79_woodmaple", "Guild X-79, Maple"));
		}
		private void AddBass()
		{
			charBass.Items.Add(new RBString("5string_paint", "Fender 5-String, Paint"));
			charBass.Items.Add(new RBString("5string_sparkle", "Fender 5-String, Sparkle"));
			charBass.Items.Add(new RBString("5string_sunburstblack", "Fender 5-String, Black Sunburst"));
			charBass.Items.Add(new RBString("5string_sunburstpearl", "Fender 5-String, Pearl Sunburst"));
			charBass.Items.Add(new RBString("5string_sunbursttortoise", "Fender 5-String, Totroise Sunburst"));
			charBass.Items.Add(new RBString("5string_sunburstwhite", "Fender 5-String, White Sunburst"));
			charBass.Items.Add(new RBString("5string_triburstblack", "Fender 5-String, Black Triburst"));
			charBass.Items.Add(new RBString("5string_triburstpearl", "Fender 5-String, Pearl Triburst"));
			charBass.Items.Add(new RBString("5string_tribursttortoise", "Fender 5-String, Tortoise Triburst"));
			charBass.Items.Add(new RBString("5string_triburstwhite", "Fender 5-String, White Triburst"));
			charBass.Items.Add(new RBString("5string_woodash", "Fender 5-String, Ash"));
			charBass.Items.Add(new RBString("5string_woodmaple", "Fender 5-String, Maple"));
			charBass.Items.Add(new RBString("axebass_resource", "The Axecutioner Bass"));
			charBass.Items.Add(new RBString("batwingbass_resource", "The Batwing Bass"));
			charBass.Items.Add(new RBString("boltbass_resource", "Lightning Bolt! Bass"));
			charBass.Items.Add(new RBString("brainbass_resource", "The Brain Bass"));
			charBass.Items.Add(new RBString("c20bass_paint", "Jackson C20 Concert, Paint"));
			charBass.Items.Add(new RBString("c20bass_sparkle", "Jackson C20 Concert, Sparkle"));
			charBass.Items.Add(new RBString("c20bass_sunburst", "Jackson C20 Concert, Sunburst"));
			charBass.Items.Add(new RBString("c20bass_triburst", "Jackson C20 Concert, Triburst"));
			charBass.Items.Add(new RBString("c20bass_woodash", "Jackson C20 Concert, Ash"));
			charBass.Items.Add(new RBString("c20bass_woodmaple", "Jackson C20 Concert, Maple"));
			charBass.Items.Add(new RBString("chainsawbass_resource", "The Chainsaw Bass"));
			charBass.Items.Add(new RBString("committeebass_paint", "Gretsch Committee, Paint"));
			charBass.Items.Add(new RBString("committeebass_sparkle", "Gretsch Committee, Sparkle"));
			charBass.Items.Add(new RBString("committeebass_sunburst", "Gretsch Committee, Sunburst"));
			charBass.Items.Add(new RBString("committeebass_triburst", "Gretsch Committee, Triburst"));
			charBass.Items.Add(new RBString("committeebass_woodash", "Gretsch Committee, Ash"));
			charBass.Items.Add(new RBString("committeebass_woodmaple", "Gretsch Committee, Maple"));
			charBass.Items.Add(new RBString("cthulhubass_resource", "Cthulhu's Revenge Bass"));
			charBass.Items.Add(new RBString("doublebass_paint", "Fender Doubleneck Custom, Paint"));
			charBass.Items.Add(new RBString("doublebass_sparkle", "Fender Doubleneck Custom, Sparkle"));
			charBass.Items.Add(new RBString("doublebass_sunburstblack", "Fender Doubleneck Custom, Black Sunburst"));
			charBass.Items.Add(new RBString("doublebass_sunburstpearl", "Fender Doubleneck Custom, Pearl Sunburst"));
			charBass.Items.Add(new RBString("doublebass_sunbursttortoise", "Fender Doubleneck Custom, Tortoise Sunburst"));
			charBass.Items.Add(new RBString("doublebass_sunburstwhite", "Fender Doubleneck Custom, White Sunburst"));
			charBass.Items.Add(new RBString("doublebass_triburstblack", "Fender Doubleneck Custom, Black Triburst"));
			charBass.Items.Add(new RBString("doublebass_triburstpearl", "Fender Doubleneck Custom, Pearl Triburst"));
			charBass.Items.Add(new RBString("doublebass_tribursttortoise", "Fender Doubleneck Custom, Tortoise Triburst"));
			charBass.Items.Add(new RBString("doublebass_triburstwhite", "Fender Doubleneck Custom, White Triburst"));
			charBass.Items.Add(new RBString("doublebass_woodash", "Fender Doubleneck Custom, Ash"));
			charBass.Items.Add(new RBString("doublebass_woodmaple", "Fender Doubleneck Custom, Maple"));
			charBass.Items.Add(new RBString("falconbass_paint", "Gretsch G6136LSB White Falcon Long Scale Hollow-Body, Paint"));
			charBass.Items.Add(new RBString("falconbass_sparkle", "Gretsch G6136LSB White Falcon Long Scale Hollow-Body, Sparkle"));
			charBass.Items.Add(new RBString("falconbass_sunburst", "Gretsch G6136LSB White Falcon Long Scale Hollow-Body, Sunburst"));
			charBass.Items.Add(new RBString("falconbass_triburst", "Gretsch G6136LSB White Falcon Long Scale Hollow-Body, Triburst"));
			charBass.Items.Add(new RBString("falconbass_woodash", "Gretsch G6136LSB White Falcon Long Scale Hollow-Body, Ash"));
			charBass.Items.Add(new RBString("falconbass_woodmaple", "Gretsch G6136LSB White Falcon Long Scale Hollow-Body, Maple"));
			charBass.Items.Add(new RBString("g6073_paint", "Gretsch G6073 Electrotone, Paint"));
			charBass.Items.Add(new RBString("g6073_sparkle", "Gretsch G6073 Electrotone, Sparkle"));
			charBass.Items.Add(new RBString("g6073_sunburst", "Gretsch G6073 Electrotone, Sunburst"));
			charBass.Items.Add(new RBString("g6073_triburst", "Gretsch G6073 Electrotone, Triburst"));
			charBass.Items.Add(new RBString("g6073_woodash", "Gretsch G6073 Electrotone, Ash"));
			charBass.Items.Add(new RBString("g6073_woodmaple", "Gretsch G6073 Electrotone, Maple"));
			charBass.Items.Add(new RBString("g6128b_paint", "Gretsch G6128B, Paint"));
			charBass.Items.Add(new RBString("g6128b_sparkle", "Gretsch G6128B, Sparkle"));
			charBass.Items.Add(new RBString("g6128b_sunburst", "Gretsch G6128B, Sunburst"));
			charBass.Items.Add(new RBString("g6128b_triburst", "Gretsch G6128B, Triburst"));
			charBass.Items.Add(new RBString("g6128b_woodash", "Gretsch G6128B, Ash"));
			charBass.Items.Add(new RBString("g6128b_woodmaple", "Gretsch G6128B, Maple"));
			charBass.Items.Add(new RBString("goatheadbass_resource", "The Goat Head Bass"));
			charBass.Items.Add(new RBString("gravebass_resource", "The Grave Bass"));
			charBass.Items.Add(new RBString("hbombbass_resource", "The Bomb Bass"));
			charBass.Items.Add(new RBString("jaguarbass_paint", "Fender Deluxe Jaguar, Paint"));
			charBass.Items.Add(new RBString("jaguarbass_sparkle", "Fender Deluxe Jaguar, Sparkle"));
			charBass.Items.Add(new RBString("jaguarbass_sunburstblack", "Fender Deluxe Jaguar, Black Sunburst"));
			charBass.Items.Add(new RBString("jaguarbass_sunburstpearl", "Fender Deluxe Jaguar, Pearl Sunburst"));
			charBass.Items.Add(new RBString("jaguarbass_sunbursttortoise", "Fender Deluxe Jaguar, Tortoise Sunburst"));
			charBass.Items.Add(new RBString("jaguarbass_sunburstwhite", "Fender Deluxe Jaguar, White Sunburst"));
			charBass.Items.Add(new RBString("jaguarbass_triburstblack", "Fender Deluxe Jaguar, Black Triburst"));
			charBass.Items.Add(new RBString("jaguarbass_triburstpearl", "Fender Deluxe Jaguar, Pearl Triburst"));
			charBass.Items.Add(new RBString("jaguarbass_tribursttortoise", "Fender Deluxe Jaguar, Tortoise Triburst"));
			charBass.Items.Add(new RBString("jaguarbass_triburstwhite", "Fender Deluxe Jaguar, White Triburst"));
			charBass.Items.Add(new RBString("jaguarbass_woodash", "Fender Deluxe Jaguar, Ash"));
			charBass.Items.Add(new RBString("jaguarbass_woodmaple", "Fender Deluxe Jaguar, Maple"));
			charBass.Items.Add(new RBString("jazz01_paint", "Fender 1962 Jazz, Paint"));
			charBass.Items.Add(new RBString("jazz01_sparkle", "Fender 1962 Jazz, Sparkle"));
			charBass.Items.Add(new RBString("jazz01_sunburstblack", "Fender 1962 Jazz, Black Sunburst"));
			charBass.Items.Add(new RBString("jazz01_sunburstpearl", "Fender 1962 Jazz, Pearl Sunburst"));
			charBass.Items.Add(new RBString("jazz01_sunbursttortoise", "Fender 1962 Jazz, Tortoise Sunburst"));
			charBass.Items.Add(new RBString("jazz01_sunburstwhite", "Fender 1962 Jazz, White Sunburst"));
			charBass.Items.Add(new RBString("jazz01_triburstblack", "Fender 1962 Jazz, Black Triburst"));
			charBass.Items.Add(new RBString("jazz01_triburstpearl", "Fender 1962 Jazz, Pearl Triburst"));
			charBass.Items.Add(new RBString("jazz01_tribursttortoise", "Fender 1962 Jazz, Tortoise Triburst"));
			charBass.Items.Add(new RBString("jazz01_triburstwhite", "Fender 1962 Jazz, White Triburst"));
			charBass.Items.Add(new RBString("jazz01_woodash", "Fender 1962 Jazz, Ash"));
			charBass.Items.Add(new RBString("jazz01_woodmaple", "Fender 1962 Jazz, Maple"));
			charBass.Items.Add(new RBString("jazz02_paint", "Fender 1975 Standard Jazz, Paint"));
			charBass.Items.Add(new RBString("jazz02_sparkle", "Fender 1975 Standard Jazz, Sparkle"));
			charBass.Items.Add(new RBString("jazz02_sunburstblack", "Fender 1975 Standard Jazz, Black Sunburst"));
			charBass.Items.Add(new RBString("jazz02_sunburstpearl", "Fender 1975 Standard Jazz, Pearl Sunburst"));
			charBass.Items.Add(new RBString("jazz02_sunbursttortoise", "Fender 1975 Standard Jazz, Tortoise Sunburst"));
			charBass.Items.Add(new RBString("jazz02_sunburstwhite", "Fender 1975 Standard Jazz, White Sunburst"));
			charBass.Items.Add(new RBString("jazz02_triburstblack", "Fender 1975 Standard Jazz, Black Triburst"));
			charBass.Items.Add(new RBString("jazz02_triburstpearl", "Fender 1975 Standard Jazz, Pearl Triburst"));
			charBass.Items.Add(new RBString("jazz02_tribursttortoise", "Fender 1975 Standard Jazz, Tortoise Triburst"));
			charBass.Items.Add(new RBString("jazz02_triburstwhite", "Fender 1975 Standard Jazz, White Triburst"));
			charBass.Items.Add(new RBString("jazz02_woodash", "Fender 1975 Standard Jazz, Ash"));
			charBass.Items.Add(new RBString("jazz02_woodmaple", "Fender 1975 Standard Jazz, Maple"));
			charBass.Items.Add(new RBString("jazz03_paint", "Fender Jazz 24, Paint"));
			charBass.Items.Add(new RBString("jazz03_sparkle", "Fender Jazz 24, Sparkle"));
			charBass.Items.Add(new RBString("jazz03_sunburst", "Fender Jazz 24, Sunburst"));
			charBass.Items.Add(new RBString("jazz03_triburst", "Fender Jazz 24, Triburst"));
			charBass.Items.Add(new RBString("jazz03_woodash", "Fender Jazz 24, Ash"));
			charBass.Items.Add(new RBString("jazz03_woodmaple", "Fender Jazz 24, Maple"));
			charBass.Items.Add(new RBString("js_paint", "Guild JS, Paint"));
			charBass.Items.Add(new RBString("js_sparkle", "Guild JS, Sparkle"));
			charBass.Items.Add(new RBString("js_sunburst", "Guild JS, Sunburst"));
			charBass.Items.Add(new RBString("js_triburst", "Guild JS, Triburst"));
			charBass.Items.Add(new RBString("js_woodash", "Guild JS, Ash"));
			charBass.Items.Add(new RBString("js_woodmaple", "Guild JS, Maple"));
			charBass.Items.Add(new RBString("kelly04_paint", "Jackson Kelly, Paint"));
			charBass.Items.Add(new RBString("kelly04_sparkle", "Jackson Kelly, Sparkle"));
			charBass.Items.Add(new RBString("kelly04_sunburst", "Jackson Kelly, Sunburst"));
			charBass.Items.Add(new RBString("kelly04_triburst", "Jackson Kelly, Triburst"));
			charBass.Items.Add(new RBString("kelly04_woodash", "Jackson Kelly, Ash"));
			charBass.Items.Add(new RBString("kelly04_woodmaple", "Jackson Kelly, Maple"));
			charBass.Items.Add(new RBString("macebass_resource", "The Mace Bass"));
			charBass.Items.Add(new RBString("magnum1_paint", "Ovation Magnum I, Paint"));
			charBass.Items.Add(new RBString("magnum1_sparkle", "Ovation Magnum I, Sparkle"));
			charBass.Items.Add(new RBString("magnum1_sunburstblack", "Ovation Magnum I, Black Sunburst"));
			charBass.Items.Add(new RBString("magnum1_sunburstpearl", "Ovation Magnum I, Pearl Sunburst"));
			charBass.Items.Add(new RBString("magnum1_sunbursttortoise", "Ovation Magnum I, Tortoise Sunburst"));
			charBass.Items.Add(new RBString("magnum1_sunburstwhite", "Ovation Magnum I, White Sunburst"));
			charBass.Items.Add(new RBString("magnum1_triburstblack", "Ovation Magnum I, Black Triburst"));
			charBass.Items.Add(new RBString("magnum1_triburstpearl", "Ovation Magnum I, Pearl Triburst"));
			charBass.Items.Add(new RBString("magnum1_tribursttortoise", "Ovation Magnum I, Tortoise Triburst"));
			charBass.Items.Add(new RBString("magnum1_triburstwhite", "Ovation Magnum I, White Triburst"));
			charBass.Items.Add(new RBString("magnum1_woodash", "Ovation Magnum I, Ash"));
			charBass.Items.Add(new RBString("magnum1_woodmaple", "Ovation Magnum I, Maple"));
			charBass.Items.Add(new RBString("magnum4_paint", "Ovation Magnum IV, Paint"));
			charBass.Items.Add(new RBString("magnum4_sparkle", "Ovation Magnum IV, Sparkle"));
			charBass.Items.Add(new RBString("magnum4_sunburstblack", "Ovation Magnum IV, Black Sunburst"));
			charBass.Items.Add(new RBString("magnum4_sunburstpearl", "Ovation Magnum IV, Pearl Sunburst"));
			charBass.Items.Add(new RBString("magnum4_sunbursttortoise", "Ovation Magnum IV, Tortoise Sunburst"));
			charBass.Items.Add(new RBString("magnum4_sunburstwhite", "Ovation Magnum IV, White Sunburst"));
			charBass.Items.Add(new RBString("magnum4_triburstblack", "Ovation Magnum IV, Black Triburst"));
			charBass.Items.Add(new RBString("magnum4_triburstpearl", "Ovation Magnum IV, Pearl Triburst"));
			charBass.Items.Add(new RBString("magnum4_tribursttortoise", "Ovation Magnum IV, Tortoise Triburst"));
			charBass.Items.Add(new RBString("magnum4_triburstwhite", "Ovation Magnum IV, White Triburst"));
			charBass.Items.Add(new RBString("magnum4_woodash", "Ovation Magnum IV, Ash"));
			charBass.Items.Add(new RBString("magnum4_woodmaple", "Ovation Magnum IV, Maple"));
			charBass.Items.Add(new RBString("mb4_paint", "Squier MB-4, Paint"));
			charBass.Items.Add(new RBString("mb4_sparkle", "Squier MB-4, Sparkle"));
			charBass.Items.Add(new RBString("mb4_sunburst", "Squier MB-4, Sunburst"));
			charBass.Items.Add(new RBString("mb4_triburst", "Squier MB-4, Triburst"));
			charBass.Items.Add(new RBString("mb4_woodash", "Squier MB-4, Ash"));
			charBass.Items.Add(new RBString("mb4_woodmaple", "Squier MB-4, Maple"));
			charBass.Items.Add(new RBString("mustang01_paint", "Fender Classic Mustang, Paint"));
			charBass.Items.Add(new RBString("mustang01_sparkle", "Fender Classic Mustang, Sparkle"));
			charBass.Items.Add(new RBString("mustang01_sunburstblack", "Fender Classic Mustang, Black Sunburst"));
			charBass.Items.Add(new RBString("mustang01_sunburstpearl", "Fender Classic Mustang, Pearl Sunburst"));
			charBass.Items.Add(new RBString("mustang01_sunbursttortoise", "Fender Classic Mustang, Tortoise Sunburst"));
			charBass.Items.Add(new RBString("mustang01_sunburstwhite", "Fender Classic Mustang, White Sunburst"));
			charBass.Items.Add(new RBString("mustang01_triburstblack", "Fender Classic Mustang, Black Triburst"));
			charBass.Items.Add(new RBString("mustang01_triburstpearl", "Fender Classic Mustang, Pearl Triburst"));
			charBass.Items.Add(new RBString("mustang01_tribursttortoise", "Fender Classic Mustang, Tortoise Triburst"));
			charBass.Items.Add(new RBString("mustang01_triburstwhite", "Fender Classic Mustang, White Triburst"));
			charBass.Items.Add(new RBString("mustang01_woodash", "Fender Classic Mustang, Ash"));
			charBass.Items.Add(new RBString("mustang01_woodmaple", "Fender Classic Mustang, Maple"));
			charBass.Items.Add(new RBString("neonbass_resource", "Neon Dream Bass"));
			charBass.Items.Add(new RBString("pablo_paint", "Jackson Custom Shop Pablo Santana Extreme, Paint"));
			charBass.Items.Add(new RBString("pablo_sparkle", "Jackson Custom Shop Pablo Santana Extreme, Sparkle"));
			charBass.Items.Add(new RBString("pablo_sunburst", "Jackson Custom Shop Pablo Santana Extreme, Sunburst"));
			charBass.Items.Add(new RBString("pablo_triburst", "Jackson Custom Shop Pablo Santana Extreme, Triburst"));
			charBass.Items.Add(new RBString("pablo_woodash", "Jackson Custom Shop Pablo Santana Extreme, Ash"));
			charBass.Items.Add(new RBString("pablo_woodmaple", "Jackson Custom Shop Pablo Santana Extreme, Maple"));
			charBass.Items.Add(new RBString("precision01_paint", "Fender American Vintage 1962 Precision, Paint"));
			charBass.Items.Add(new RBString("precision01_sparkle", "Fender American Vintage 1962 Precision, Sparkle"));
			charBass.Items.Add(new RBString("precision01_sunburstblack", "Fender American Vintage 1962 Precision, Black Sunburst"));
			charBass.Items.Add(new RBString("precision01_sunburstpearl", "Fender American Vintage 1962 Precision, Pearl Sunburst"));
			charBass.Items.Add(new RBString("precision01_sunbursttortoise", "Fender American Vintage 1962 Precision, Tortoise Sunburst"));
			charBass.Items.Add(new RBString("precision01_sunburstwhite", "Fender American Vintage 1962 Precision, White Sunburst"));
			charBass.Items.Add(new RBString("precision01_triburstblack", "Fender American Vintage 1962 Precision, Black Triburst"));
			charBass.Items.Add(new RBString("precision01_triburstpearl", "Fender American Vintage 1962 Precision, Pearl Triburst"));
			charBass.Items.Add(new RBString("precision01_tribursttortoise", "Fender American Vintage 1962 Precision, Tortoise Triburst"));
			charBass.Items.Add(new RBString("precision01_triburstwhite", "Fender American Vintage 1962 Precision, White Triburst"));
			charBass.Items.Add(new RBString("precision01_woodash", "Fender American Vintage 1962 Precision, Ash"));
			charBass.Items.Add(new RBString("precision01_woodmaple", "Fender American Vintage 1962 Precision, Maple"));
			charBass.Items.Add(new RBString("precision02_paint", "Fender Big Block Precision, Paint"));
			charBass.Items.Add(new RBString("precision02_sparkle", "Fender Big Block Precision, Sparkle"));
			charBass.Items.Add(new RBString("precision02_sunburst", "Fender Big Block Precision, Sunburst"));
			charBass.Items.Add(new RBString("precision02_triburst", "Fender Big Block Precision, Triburst"));
			charBass.Items.Add(new RBString("precision02_woodash", "Fender Big Block Precision, Ash"));
			charBass.Items.Add(new RBString("precision02_woodmaple", "Fender Big Block Precision, Maple"));
			charBass.Items.Add(new RBString("precision04_paint", "Fender 1951 Precision, Paint"));
			charBass.Items.Add(new RBString("precision04_sparkle", "Fender 1951 Precision, Sparkle"));
			charBass.Items.Add(new RBString("precision04_sunburstblack", "Fender 1951 Precision, Black Sunburst"));
			charBass.Items.Add(new RBString("precision04_sunburstpearl", "Fender 1951 Precision, Pearl Sunburst"));
			charBass.Items.Add(new RBString("precision04_sunbursttortoise", "Fender 1951 Precision, Tortoise Sunburst"));
			charBass.Items.Add(new RBString("precision04_sunburstwhite", "Fender 1951 Precision, White Sunburst"));
			charBass.Items.Add(new RBString("precision04_triburstblack", "Fender 1951 Precision, Black Triburst"));
			charBass.Items.Add(new RBString("precision04_triburstpearl", "Fender 1951 Precision, Pearl Triburst"));
			charBass.Items.Add(new RBString("precision04_tribursttortoise", "Fender 1951 Precision, Tortoise Triburst"));
			charBass.Items.Add(new RBString("precision04_triburstwhite", "Fender 1951 Precision, White Triburst"));
			charBass.Items.Add(new RBString("precision04_woodash", "Fender 1951 Precision, Ash"));
			charBass.Items.Add(new RBString("precision04_woodmaple", "Fender 1951 Precision, Maple"));
			charBass.Items.Add(new RBString("precision05_paint", "Fender American Standard Precision, Paint"));
			charBass.Items.Add(new RBString("precision05_sparkle", "Fender American Standard Precision, Sparkle"));
			charBass.Items.Add(new RBString("precision05_sunburstblack", "Fender American Standard Precision, Black Sunburst"));
			charBass.Items.Add(new RBString("precision05_sunburstpearl", "Fender American Standard Precision, Pearl Sunburst"));
			charBass.Items.Add(new RBString("precision05_sunbursttortoise", "Fender American Standard Precision, Tortoise Sunburst"));
			charBass.Items.Add(new RBString("precision05_sunburstwhite", "Fender American Standard Precision, White Sunburst"));
			charBass.Items.Add(new RBString("precision05_triburstblack", "Fender American Standard Precision, Black Triburst"));
			charBass.Items.Add(new RBString("precision05_triburstpearl", "Fender American Standard Precision, Pearl Triburst"));
			charBass.Items.Add(new RBString("precision05_tribursttortoise", "Fender American Standard Precision, Tortoise Triburst"));
			charBass.Items.Add(new RBString("precision05_triburstwhite", "Fender American Standard Precision, White Triburst"));
			charBass.Items.Add(new RBString("precision05_woodash", "Fender American Standard Precision, Ash"));
			charBass.Items.Add(new RBString("precision05_woodmaple", "Fender American Standard Precision, Maple"));
			charBass.Items.Add(new RBString("prefishbass_resource", "Bubbles Bass"));
			charBass.Items.Add(new RBString("skeletarbass_resource", "SkeleTone Bass"));
			charBass.Items.Add(new RBString("skullbass_resource", "The Skull Bass"));
			charBass.Items.Add(new RBString("teardropbass_resource", "The Teardrop Bass"));
			charBass.Items.Add(new RBString("telebass_paint", "Squier Vintage Modified Precision, Paint"));
			charBass.Items.Add(new RBString("telebass_sparkle", "Squier Vintage Modified Precision, Sparkle"));
			charBass.Items.Add(new RBString("telebass_sunburstblack", "Squier Vintage Modified Precision, Black Sunburst"));
			charBass.Items.Add(new RBString("telebass_sunburstpearl", "Squier Vintage Modified Precision, Pearl Sunburst"));
			charBass.Items.Add(new RBString("telebass_sunbursttortoise", "Squier Vintage Modified Precision, Tortoise Sunburst"));
			charBass.Items.Add(new RBString("telebass_sunburstwhite", "Squier Vintage Modified Precision, White Sunburst"));
			charBass.Items.Add(new RBString("telebass_triburstblack", "Squier Vintage Modified Precision, Black Triburst"));
			charBass.Items.Add(new RBString("telebass_triburstpearl", "Squier Vintage Modified Precision, Pearl Triburst"));
			charBass.Items.Add(new RBString("telebass_tribursttortoise", "Squier Vintage Modified Precision, Tortoise Triburst"));
			charBass.Items.Add(new RBString("telebass_triburstwhite", "Squier Vintage Modified Precision, White Triburst"));
			charBass.Items.Add(new RBString("telebass_woodash", "Squier Vintage Modified Precision, Ash"));
			charBass.Items.Add(new RBString("telebass_woodmaple", "Squier Vintage Modified Precision, Maple"));
			charBass.Items.Add(new RBString("thehandbass_resource", "Hand of Gargalonn Bass"));
			charBass.Items.Add(new RBString("typhoon2_paint", "Ovation Typhoon II, Paint"));
			charBass.Items.Add(new RBString("typhoon2_sparkle", "Ovation Typhoon II, Sparkle"));
			charBass.Items.Add(new RBString("typhoon2_sunburst", "Ovation Typhoon II, Sunburst"));
			charBass.Items.Add(new RBString("typhoon2_triburst", "Ovation Typhoon II, Triburst"));
			charBass.Items.Add(new RBString("typhoon2_woodash", "Ovation Typhoon II, Ash"));
			charBass.Items.Add(new RBString("typhoon2_woodmaple", "Ovation Typhoon II, Maple"));
			charBass.Items.Add(new RBString("velocity_paint", "Hamer Velocity 2, Paint"));
			charBass.Items.Add(new RBString("velocity_sparkle", "Hamer Velocity 2, Sparkle"));
			charBass.Items.Add(new RBString("velocity_sunburst", "Hamer Velocity 2, Sunburst"));
			charBass.Items.Add(new RBString("velocity_triburst", "Hamer Velocity 2, Triburst"));
			charBass.Items.Add(new RBString("velocity_woodash", "Hamer Velocity 2, Ash"));
			charBass.Items.Add(new RBString("velocity_woodmaple", "Hamer Velocity 2, Maple"));
			charBass.Items.Add(new RBString("wildwoodbass_paint", "Fender Wildwood, Paint"));
			charBass.Items.Add(new RBString("wildwoodbass_sparkle", "Fender Wildwood, Sparkle"));
			charBass.Items.Add(new RBString("wildwoodbass_sunburst", "Fender Wildwood, Sunburst"));
			charBass.Items.Add(new RBString("wildwoodbass_triburst", "Fender Wildwood, Triburst"));
			charBass.Items.Add(new RBString("wildwoodbass_woodash", "Fender Wildwood, Ash"));
			charBass.Items.Add(new RBString("wildwoodbass_woodmaple", "Fender Wildwood, Maple"));
			charBass.Items.Add(new RBString("wingsbass_resource", "The Fallen Angel Bass"));
		}
		private void AddDrum()
		{
			charDrum.Items.Add(new RBString("dw_billiards", "DW Collector's Series, Billiards"));
			charDrum.Items.Add(new RBString("dw_blackjack", "DW Collector's Series, Blackjack"));
			charDrum.Items.Add(new RBString("dw_dwburst", "DW Collector's Series, Burst"));
			charDrum.Items.Add(new RBString("dw_dwfade", "DW Collector's Series, Fade"));
			charDrum.Items.Add(new RBString("dw_dwoyster", "DW Collector's Series, Oyster"));
			charDrum.Items.Add(new RBString("dw_flamehotrod", "DW Collector's Series, Hot Rod"));
			charDrum.Items.Add(new RBString("dw_flametribalnatural", "DW Collector's Series, Tribal Natural"));
			charDrum.Items.Add(new RBString("dw_flametribal", "DW Collector's Series, Tribal"));
			charDrum.Items.Add(new RBString("dw_glass", "DW Collector's Series, Glass"));
			charDrum.Items.Add(new RBString("dw_marine", "DW Collector's Series, Marine"));
			charDrum.Items.Add(new RBString("dw_natural", "DW Collector's Series, Maple"));
			charDrum.Items.Add(new RBString("dw_rallystripe", "DW Collector's Series, Rally Stripe"));
			charDrum.Items.Add(new RBString("dw_tribalband", "DW Collector's Series, Tribal band"));
			charDrum.Items.Add(new RBString("electronic", "DKS-5910 Pro High-Tech Performance Kit"));
			charDrum.Items.Add(new RBString("generic_leopard", "HMX V-Series, Leopard"));
			charDrum.Items.Add(new RBString("generic_snake", "HMX V-Series, Snake"));
			charDrum.Items.Add(new RBString("generic_steel", "HMX V-Series, Steel"));
			charDrum.Items.Add(new RBString("generic_stone", "HMX V-Series, Stone"));
			charDrum.Items.Add(new RBString("generic_zebra", "HMX V-Series, Zebra"));
			charDrum.Items.Add(new RBString("goth_gold", "Baroque Stage Kit, Gold"));
			charDrum.Items.Add(new RBString("goth_iron", "Baroque Stage Kit, Iron"));
			charDrum.Items.Add(new RBString("ludclassic_cortex", "Ludwig Classic, Cortex"));
			charDrum.Items.Add(new RBString("ludclassic_diamond", "Ludwig Classic, Diamond"));
			charDrum.Items.Add(new RBString("ludclassic_fade", "Ludwig Classic, Fade"));
			charDrum.Items.Add(new RBString("ludclassic_ludwigburst", "Ludwig Classic, Burst"));
			charDrum.Items.Add(new RBString("ludclassic_ludwigoyster", "Ludwig Classic, Oyster"));
			charDrum.Items.Add(new RBString("ludclassic_naturalmaple", "Ludwig Classic, Maple"));
			charDrum.Items.Add(new RBString("ludclassic_sparkle", "Ludwig Classic, Sparkle"));
			charDrum.Items.Add(new RBString("ludvista_2bandsolid", "Ludwig Vistalite, 2 Band Solid"));
			charDrum.Items.Add(new RBString("ludvista_2bandtrans", "Ludwig Vistalite, 2 Band Transparent"));
			charDrum.Items.Add(new RBString("ludvista_3bandsolid", "Ludwig Vistalite, 3 Band Solid"));
			charDrum.Items.Add(new RBString("ludvista_3bandtrans", "Ludwig Vistalite, 3 Band Transparent"));
			charDrum.Items.Add(new RBString("ludvista_3rainbowsolid", "Ludwig Vistalite, 3 Rainbow Solid"));
			charDrum.Items.Add(new RBString("ludvista_3rainbowtrans", "Ludwig Vistalite, 3 Rainbow Transparent"));
			charDrum.Items.Add(new RBString("ludvista_5rainbowsolid", "Ludwig Vistalite, 5 Rainbox Solid"));
			charDrum.Items.Add(new RBString("ludvista_5rainbowtrans", "Ludwig Vistalite, 5 Rainbox Transparent"));
			charDrum.Items.Add(new RBString("ludvista_chrome", "Ludwig Vistalite, Chrome"));
			charDrum.Items.Add(new RBString("ludvista_clear", "Ludwig Vistalite, Clear"));
			charDrum.Items.Add(new RBString("ludvista_spiralsolid", "Ludwig Vistalite, Spiral Solid"));
			charDrum.Items.Add(new RBString("ludvista_spiraltrans", "Ludwig Vistalite, Spiral Transparent"));
			charDrum.Items.Add(new RBString("ludvista_verticalsolid", "Ludwig Vistalite, Bar Solid"));
			charDrum.Items.Add(new RBString("ludvista_verticaltrans", "Ludwig Vistalite, Bar Transparent"));
			charDrum.Items.Add(new RBString("pearl_burst", "Pearl Reference Series, Burst"));
			charDrum.Items.Add(new RBString("pearl_fade", "Pearl Reference Series, Fade"));
			charDrum.Items.Add(new RBString("pearl_lacquer", "Pearl Reference Series, Lacquer"));
			charDrum.Items.Add(new RBString("pearl_maple", "Pearl Reference Series, Maple"));
			charDrum.Items.Add(new RBString("pearl_pearlescent", "Pearl Reference Series, Pearlescent"));
			charDrum.Items.Add(new RBString("pearl_sparklefade", "Pearl Reference Series, Sparkle Fade"));
			charDrum.Items.Add(new RBString("pearl_sparkle", "Pearl Reference Series, Sparkle"));
			charDrum.Items.Add(new RBString("rewarddrumclear", "Clear Kit"));
			charDrum.Items.Add(new RBString("rewarddrumgold", "Gold Kit"));
			charDrum.Items.Add(new RBString("rewarddrumsilver", "Silver Kit"));
			charDrum.Items.Add(new RBString("rust", "Warehouse Original Kit"));
			charDrum.Items.Add(new RBString("trash", "Hobo Crate Kit"));
		}
		private void AddMic()
		{
			charMic.Items.Add(new RBString("beta57_resource", "Shure Beta 57"));
			charMic.Items.Add(new RBString("beta87_resource", "Shure Beta 87A"));
			charMic.Items.Add(new RBString("bonesandspikes_resource", "The MicroBone"));
			charMic.Items.Add(new RBString("brokenmic_resource", "Sonic Transducer"));
			charMic.Items.Add(new RBString("e935_resource", "Sennheiser e 935"));
			charMic.Items.Add(new RBString("goth_resource", "The Claw"));
			charMic.Items.Add(new RBString("kms104_resource", "Neumann KMS 104"));
			charMic.Items.Add(new RBString("knife_resource", "Studded Belter"));
			charMic.Items.Add(new RBString("ksm9wireless_resource", "Shure KSM9 Wireless Transmitter"));
			charMic.Items.Add(new RBString("md431ii_resource", "Sennheiser MD 431 II"));
			charMic.Items.Add(new RBString("md441u_resource", "Sennheiser MD 441"));
			charMic.Items.Add(new RBString("mic1_clear", "Clear Microphone"));
			charMic.Items.Add(new RBString("mic1_gold", "Gold Microphone"));
			charMic.Items.Add(new RBString("mic1_silver", "Silver Microphone"));
			charMic.Items.Add(new RBString("sm57_resource", "Shure SM57"));
			charMic.Items.Add(new RBString("sm58_resource", "Shure SM58"));
		}
		private void AddKeys()
		{
			charKeys.Items.Add(new RBString("bx3_resource", "Korg BX-3"));
			charKeys.Items.Add(new RBString("cs80_resource", "Yamaha CS-80"));
			charKeys.Items.Add(new RBString("dx7_resource", "Yamaha DX7"));
			charKeys.Items.Add(new RBString("m3_resource", "Korg M3"));
			charKeys.Items.Add(new RBString("m50_resource", "Korg M50"));
			charKeys.Items.Add(new RBString("microkorg_resource", "Korg microKORG XL"));
			charKeys.Items.Add(new RBString("motif_resource", "Yamaha MOTIF XS"));
			charKeys.Items.Add(new RBString("polyevolver_resource", "Dave Smith Poly Evolver PE"));
			charKeys.Items.Add(new RBString("prophet_resource", "Dave Smith Prophet '08"));
			charKeys.Items.Add(new RBString("radias_resource", "Korg RADIAS"));
			charKeys.Items.Add(new RBString("vox_resource", "VOX Continental"));
			charKeys.Items.Add(new RBString("xk3_resource", "Hammond XK-3c"));
		}
		#endregion

		public class RBString
		{
			public string saveString { get; set; }
			public string localeString { get; set; }

			public RBString(string save, string locale)
			{
				saveString = save;
				localeString = locale;
			}

			public override int GetHashCode()
			{
				return saveString.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == this) return true;
				var other = obj as RBString;
				if (other == null) return false;
				return saveString == other.saveString;
			}
		}

		#region valuechanged
		private void FaceShape_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var upDown = (Xceed.Wpf.Toolkit.IntegerUpDown)sender;
			var shapeImg = new Image();

			if (e.NewValue == null)
			{
				upDown.Value = 1;
				return;
			}

			string shape = "";
			switch (upDown.Name)
			{
				case "charCheek":
				{
					shape = "_shape_";
					shapeImg = charCheekImg;
					break;
				}
				case "charChin":
				{
					shape = "_chin_";
					shapeImg = charChinImg;
					break;
				}
				case "charNose":
				{
					shape = "_nose_";
					shapeImg = charNoseImg;
					break;
				}
				case "charEye":
				{
					shape = "_eye_";
					shapeImg = charEyeImg;
					break;
				}
				case "charMouth":
				{
					shape = "_mouth_";
					shapeImg = charMouthImg;
					break;
				}
				default:
				return;		
			}
			shapeImg.Source = new BitmapImage(setFaceImageUri((int)e.NewValue - 1, shape));
		}

		private void SingleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue == null)
			{
				var upDown = (Xceed.Wpf.Toolkit.SingleUpDown)sender;
				upDown.Value = (float)0;
			}
		}
		#endregion

		private Uri setFaceImageUri(int value, string shape)
		{
			string newURI = "Face Images\\" + genderString + shape + value.ToString() + "_keep.png";
			return (new Uri(newURI, UriKind.Relative));
		}
	}
}
