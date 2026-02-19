using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalColors
{
	static GlobalColors()
	{
		GlobalColors.buttonColorData = new Dictionary<UIButtonOverlayOff.ButtonType, ButtonColorScheme>
		{
			{
				UIButtonOverlayOff.ButtonType.Custom,
				default(ButtonColorScheme)
			},
			{
				UIButtonOverlayOff.ButtonType.Primary,
				new ButtonColorScheme
				{
					light = new Color32?(GlobalColors.PRIMARY_ACTION_COLOR_LIGHT),
					selected = new Color32?(GlobalColors.PRIMARY_ACTION_COLOR_SELECTED),
					unavailable = new Color32?(GlobalColors.PRIMARY_ACTION_COLOR_UNAVAILABLE),
					original = new Color32?(GlobalColors.PRIMARY_ACTION_COLOR)
				}
			},
			{
				UIButtonOverlayOff.ButtonType.Tertiary,
				new ButtonColorScheme
				{
					light = new Color32?(GlobalColors.TERTIARY_ACTION_COLOR_LIGHT),
					selected = new Color32?(GlobalColors.TERTIARY_ACTION_COLOR_SELECTED),
					unavailable = new Color32?(GlobalColors.TERTIARY_ACTION_COLOR_UNAVAILABLE),
					original = new Color32?(GlobalColors.TERTIARY_ACTION_COLOR)
				}
			},
			{
				UIButtonOverlayOff.ButtonType.Footer_shop,
				new ButtonColorScheme
				{
					light = new Color32?(GlobalColors.FOOTER_ACTION_COLOR_LIGHT),
					selected = new Color32?(GlobalColors.FOOTER_ACTION_COLOR_SELECTED),
					unavailable = new Color32?(GlobalColors.FOOTER_ACTION_COLOR_UNAVAILABLE),
					original = new Color32?(GlobalColors.FOOTER_ACTION_COLOR)
				}
			}
		};
	}

	public static ButtonColorScheme GetButtonColorScheme(UIButtonOverlayOff.ButtonType buttonType, GameObject gameObject)
	{
		if (GlobalColors.buttonColorData.ContainsKey(buttonType))
		{
			return GlobalColors.buttonColorData[buttonType];
		}
		UnityEngine.Debug.LogError("No color scheme for button type: " + buttonType.ToString() + ". GameObject: " + gameObject.name);
		return GlobalColors.buttonColorData[UIButtonOverlayOff.ButtonType.Custom];
	}

	public static readonly Dictionary<UIButtonOverlayOff.ButtonType, ButtonColorScheme> buttonColorData;

	public static readonly Color32 PRIMARY_ACTION_COLOR = new Color32(70, 157, 43, byte.MaxValue);

	private static readonly Color32 PRIMARY_ACTION_COLOR_LIGHT = new Color32(139, 194, 62, byte.MaxValue);

	private static readonly Color32 PRIMARY_ACTION_COLOR_SELECTED = new Color32(54, 137, 190, byte.MaxValue);

	public static readonly Color32 PRIMARY_ACTION_COLOR_UNAVAILABLE = new Color32(118, 118, 118, byte.MaxValue);

	public static readonly Color32 TERTIARY_ACTION_COLOR = new Color32(54, 137, 190, byte.MaxValue);

	private static readonly Color32 TERTIARY_ACTION_COLOR_LIGHT = new Color32(183, 227, 245, 112);

	private static readonly Color32 TERTIARY_ACTION_COLOR_SELECTED = new Color32(54, 137, 190, byte.MaxValue);

	public static readonly Color32 TERTIARY_ACTION_COLOR_UNAVAILABLE = new Color32(118, 118, 118, byte.MaxValue);

	public static readonly Color32 FOOTER_ACTION_COLOR = new Color32(239, 239, 239, byte.MaxValue);

	private static readonly Color32 FOOTER_ACTION_COLOR_LIGHT = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private static readonly Color32 FOOTER_ACTION_COLOR_SELECTED = new Color32(byte.MaxValue, 174, 0, byte.MaxValue);

	public static readonly Color32 FOOTER_ACTION_COLOR_UNAVAILABLE = new Color32(118, 118, 118, byte.MaxValue);
}
