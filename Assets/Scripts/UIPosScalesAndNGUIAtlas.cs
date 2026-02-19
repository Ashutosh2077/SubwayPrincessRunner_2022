using System;
using UnityEngine;

public class UIPosScalesAndNGUIAtlas : MonoBehaviour
{
	public static UIPosScalesAndNGUIAtlas Instance
	{
		get
		{
			if (UIPosScalesAndNGUIAtlas._instance == null)
			{
				UIPosScalesAndNGUIAtlas._instance = UnityEngine.Object.FindObjectOfType<UIPosScalesAndNGUIAtlas>();
			}
			return UIPosScalesAndNGUIAtlas._instance;
		}
	}

	private static UIPosScalesAndNGUIAtlas _instance;

	public Vector3 characterScreenSelectedButtonPos = new Vector3(0f, -145f, 0f);

	public Vector3 characterScreenCharacterModelLocalPos = new Vector3(0f, -40f, 0f);

	public Vector3 characterScreenCharacterModelLocalScl = new Vector3(22f, 22f, 22f);

	public Vector3 characterScreenCharacterModelLocalRot = new Vector3(0f, 180f, 0f);

	public Vector3 helmetScreenCharacterModelLocalPos = Vector3.zero;

	public Vector3 helmetScreenCharacterModelLocalScl = new Vector3(22f, 22f, 22f);

	public Vector3 helmetScreenCharacterModelLocalRot = new Vector3(20f, 154.5f, 358f);

	public Vector3 celebrationCharacterUnlockCharacterModelLocalPos = Vector3.zero;

	public Vector3 celebrationCharacterUnlockCharacterModelLocalScl = new Vector3(22f, 22f, 22f);

	public Vector3 celebrationCharacterUnlockCharacterModelLocalRot = new Vector3(0f, 180f, 0f);

	public Vector3 celebrationHelmUnlockCharacterModelLocalPos = new Vector3(0f, -90f, 0f);

	public Vector3 celebrationHelmUnlockCharacterModelLocalScl = new Vector3(22f, 22f, 22f);

	public Vector3 celebrationHelmUnlockCharacterModelLocalRot = new Vector3(0f, 60f, 0f);

	public Vector3 celebrationHighScoreCharacterModelLocalPos = new Vector3(0f, -90f, 0f);

	public Vector3 celebrationHighScoreCharacterModelLocalScl = new Vector3(22f, 22f, 22f);

	public Vector3 celebrationHighScoreCharacterModelLocalRot = new Vector3(0f, 60f, 0f);

	public Vector3 tryCharacterModelLocalPos = new Vector3(0f, -100f, 0f);

	public Vector3 tryCharacterModelLocalScl = new Vector3(18f, 18f, 18f);

	public Vector3 tryCharacterModelLocalRot = new Vector3(0f, 180f, 0f);

	public string coin = "jinbi";

	public string key = "diamond";

	public string fillSpriteNameFormat = "AX_{0}Btn";

	public string slotFormat = "AX_slot{0}";

	public Color dayLblActive;

	public Color rewardLblActive;

	public Color dayLblInactive;

	public Color rewardLblInactive;

	public string achievementCellFillSpriteName = "AX_{0}Btn";

	public Color ingameHighScorePointsColor = new Color(1f, 0.8588235f, 0f, 1f);

	public Color ingameHighScoreScoreTxtColor = new Color(1f, 0.8588235f, 0f, 1f);

	public int freeViewCoinReward = 400;

	public int freeViewGemReward = 2;
}
