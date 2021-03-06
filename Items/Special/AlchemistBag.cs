﻿using ContainerLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace PortableStorage.Items.Special
{
	public class AlchemistBag : BaseBag
	{
		public override string Texture => "PortableStorage/Textures/Items/AlchemistBag";

		public AlchemistBag()
		{
			Handler = new ItemHandler(81);
			Handler.OnContentsChanged += (slot, user) =>
			{
				Recipe.FindRecipes();
				item.SyncBag();
			};
			Handler.IsItemValid += (slot, item) =>
			{
				if (slot < 18)
				{
					return
						(item.potion && item.healLife > 0 ||
						 item.healMana > 0 && !item.potion ||
						 item.buffType > 0 && !item.summon && item.buffType != BuffID.Rudolph) && !ItemID.Sets.NebulaPickup[item.type] && !IsPetItem(item);
				}

				return Utility.AlchemistBagWhitelist.Contains(item.type);
			};
		}

		private static bool IsPetItem(Item item)
		{
			bool checkItem = item.type > 0 && item.shoot > 0;
			bool checkBuff = item.buffType > 0 && item.buffType < Main.vanityPet.Length;
			if (checkItem)
			{
				checkBuff = Main.vanityPet[item.buffType] || Main.lightPet[item.buffType];
			}

			return checkItem && checkBuff;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 10);
			recipe.AddIngredient(ItemID.FossilOre, 15);
			recipe.AddIngredient(ItemID.AlchemyTable);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(mod, "PortableStorage:BagTooltip", Language.GetText("Mods.PortableStorage.BagTooltip." + GetType().Name).Format(18, 63)));
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.rare = ItemRarityID.Orange;
			item.value = 30000 * 5;
		}
	}
}