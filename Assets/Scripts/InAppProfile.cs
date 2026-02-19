using System;

public class InAppProfile
{
	public static InAppProfile getInAppProfileClone(InAppProfile objectToClone)
	{
		return new InAppProfile
		{
			amountOfCoins = objectToClone.amountOfCoins,
			amountOfKeys = objectToClone.amountOfKeys,
			amountOfHelmets = objectToClone.amountOfHelmets,
			amountOfHeadstarts = objectToClone.amountOfHeadstarts,
			amountOfScoreboosters = objectToClone.amountOfScoreboosters,
			title = objectToClone.title,
			description = objectToClone.description,
			iconName = objectToClone.iconName,
			type = objectToClone.type,
			price = objectToClone.price,
			validInApp = objectToClone.validInApp,
			isDynamicId = objectToClone.isDynamicId
		};
	}

	public int amountOfCoins;

	public int amountOfKeys;

	public int amountOfHelmets;

	public int amountOfHeadstarts;

	public int amountOfScoreboosters;

	public string title;

	public string description;

	public string iconName = string.Empty;

	public InAppData.DataType type;

	public bool isDynamicId;

	public bool isConsumable;

	public string price = "Buy";

	public bool validInApp = true;
}
