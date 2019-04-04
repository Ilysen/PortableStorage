using System;
using System.Collections.Generic;
using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.Global;
using PortableStorage.Items.Bags;
using PortableStorage.UI.Bags;
using Terraria;

namespace PortableStorage.UI
{
	public class PanelUI : BaseUI
	{
		private static Dictionary<Type, Type> UICache;

		public PanelUI()
		{
			UICache = new Dictionary<Type, Type>();

			foreach (Type type in PortableStorage.Instance.Code.GetTypes())
			{
				if (type.IsSubclassOfRawGeneric(typeof(BaseBagPanel<>)) && type.BaseType != null && type.BaseType.GenericTypeArguments.Length > 0) UICache[type.BaseType.GenericTypeArguments[0]] = type;
			}
		}

		public override void OnInitialize()
		{
		}

		public void HandleUI(BaseBag bag)
		{
			if (bag.UI != null || PortableStorage.Instance.bagCache.Contains(bag)) CloseUI(bag);
			else
			{
				Main.playerInventory = true;
				OpenUI(bag);
			}
		}

		public void CloseUI(BaseBag bag)
		{
			BaseElement element = (BaseElement)bag.UI;

			ContainerLibrary.ContainerLibrary.elements.Remove((IItemHandlerUI)element);
			Main.LocalPlayer.GetModPlayer<PSPlayer>().UIPositions[bag.ID] = element.Position;
			Elements.Remove(element);
			bag.UI = null;

			Main.PlaySound(bag.CloseSound);
		}

		public void OpenUI(BaseBag bag)
		{
			Type bagType = UICache.ContainsKey(bag.GetType()) ? bag.GetType() : bag.GetType().BaseType;

			bag.UI = (IBagPanel)Activator.CreateInstance(UICache[bagType]);
			bag.UI.ID = bag.ID;

			BaseElement element = (BaseElement)bag.UI;

			element.Activate();

			if (Main.LocalPlayer.GetModPlayer<PSPlayer>().UIPositions.TryGetValue(bag.ID, out Vector2 position))
			{
				element.HAlign = element.VAlign = 0;
				element.Position = position;
			}

			Append(element);
			ContainerLibrary.ContainerLibrary.elements.Add((IItemHandlerUI)element);

			Main.PlaySound(bag.OpenSound);
		}
	}
}