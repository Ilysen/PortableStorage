﻿using PortableStorage.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Base.UI.Elements;
using TheOneLibrary.Storage;
using TheOneLibrary.UI.Elements;
using TheOneLibrary.Utility;

namespace PortableStorage.UI
{
	public class AmmoBeltUI : BaseUI
	{
		public UIText textLabel = new UIText("Ammo Belt");

		public UIHoverButton buttonQuickStack = new UIHoverButton(Main.chestStackTexture);
		public UIButton buttonLootAll = new UIButton(PortableStorage.lootAll);
		public UIButton buttonDepositAll = new UIButton(PortableStorage.depositAll);
		public UITextButton buttonClose = new UITextButton("X", 4);

		public UIGrid gridItems = new UIGrid(9);

		public AmmoBelt ammoBelt;

		public override void OnInitialize()
		{
			panelMain.Width.Pixels = 408;
			panelMain.Height.Pixels = 172;
			panelMain.Center();
			panelMain.SetPadding(0);
			panelMain.BackgroundColor = panelColor;
			panelMain.OnMouseDown += DragStart;
			panelMain.OnMouseUp += DragEnd;
			Append(panelMain);

			textLabel.HAlign = 0.5f;
			textLabel.Top.Pixels = 8;
			panelMain.Append(textLabel);

			buttonQuickStack.Width.Pixels = 24;
			buttonQuickStack.Height.Pixels = 24;
			buttonQuickStack.Left.Pixels = 8;
			buttonQuickStack.Top.Pixels = 8;
			buttonQuickStack.HoverText = Language.GetTextValue("GameUI.QuickStackToNearby");
			buttonQuickStack.OnClick += QuickStackClick;
			panelMain.Append(buttonQuickStack);

			buttonLootAll.Width.Pixels = 24;
			buttonLootAll.Height.Pixels = 24;
			buttonLootAll.Left.Pixels = 40;
			buttonLootAll.Top.Pixels = 8;
			buttonLootAll.HoverText = Language.GetTextValue("LegacyInterface.29");
			buttonLootAll.OnClick += LootAllClick;
			panelMain.Append(buttonLootAll);

			buttonDepositAll.Width.Pixels = 24;
			buttonDepositAll.Height.Pixels = 24;
			buttonDepositAll.Left.Pixels = 72;
			buttonDepositAll.Top.Pixels = 8;
			buttonDepositAll.HoverText = Language.GetTextValue("LegacyInterface.30");
			buttonDepositAll.OnClick += DepositAllClick;
			panelMain.Append(buttonDepositAll);

			buttonClose.Width.Pixels = 24;
			buttonClose.Height.Pixels = 24;
			buttonClose.Left.Set(-28, 1);
			buttonClose.Top.Pixels = 8;
			buttonClose.OnClick += (evt, element) =>
			{
				PortableStorage.Instance.BagUI.Remove(ammoBelt.guid);
				Main.PlaySound(SoundID.Item59.WithVolume(0.5f));
			};
			panelMain.Append(buttonClose);

			gridItems.Width.Set(-16, 1);
			gridItems.Height.Set(-44, 1);
			gridItems.Left.Pixels = 8;
			gridItems.Top.Pixels = 36;
			gridItems.ListPadding = 4;
			gridItems.OverflowHidden = true;
			panelMain.Append(gridItems);
		}

		private void LootAllClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
			{
				Utility.LootAll(ammoBelt);
				Recipe.FindRecipes();
			}
		}

		public static void DepositAll(IContainerItem container)
		{
			Player player = Main.LocalPlayer;
			IList<Item> Items = container.GetItems();

			for (int pIndex = 49; pIndex >= 10; pIndex--)
			{
				Item pItem = player.inventory[pIndex];

				if (pItem.stack > 0 && pItem.type > 0 && !pItem.favorited && pItem.ammo > 0)
				{
					if (pItem.maxStack > 1)
					{
						for (int bIndex = 0; bIndex < Items.Count; bIndex++)
						{
							if (Items[bIndex].stack < Items[bIndex].maxStack && pItem.IsTheSameAs(Items[bIndex]))
							{
								int stack = pItem.stack;
								if (pItem.stack + Items[bIndex].stack > Items[bIndex].maxStack) stack = Items[bIndex].maxStack - Items[bIndex].stack;

								pItem.stack -= stack;
								Items[bIndex].stack += stack;
								Main.PlaySound(7);

								if (pItem.stack <= 0)
								{
									pItem.SetDefaults();
									break;
								}
								if (Items[bIndex].type == 0)
								{
									Items[bIndex] = pItem.Clone();
									pItem.SetDefaults();
								}
							}
						}
					}
					if (pItem.stack > 0)
					{
						for (int bIndex = 0; bIndex < Items.Count; bIndex++)
						{
							if (Items[bIndex].stack == 0)
							{
								Main.PlaySound(7);
								Items[bIndex] = pItem.Clone();
								pItem.SetDefaults();
								break;
							}
						}
					}
				}
			}
		}

		private void DepositAllClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
			{
				DepositAll(ammoBelt);
				Recipe.FindRecipes();
			}
		}

		private void QuickStackClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
			{
				Utility.QuickStack(ammoBelt);
				Recipe.FindRecipes();
			}
		}

		public void Load(AmmoBelt belt)
		{
			ammoBelt = belt;

			for (int i = 0; i < belt.GetItems().Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(belt, i);
				slot.CanInteract += (item, mouse) => mouse.IsAir || mouse.ammo > 0;
				gridItems.Add(slot);
			}
		}
	}
}