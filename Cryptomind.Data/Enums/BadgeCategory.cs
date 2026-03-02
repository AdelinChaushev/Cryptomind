namespace Cryptomind.Data.Enums
{
	public enum BadgeCategory
	{
		OnSolve = 0,      // Triggered when user solves a cipher (includes points-based badges)
		OnSuggesting = 1, // Triggered when admin approves an answer suggested by a user
		OnUpload = 2,     // Triggered when user uploads a cipher
		OnHintRequesting = 3,
	}
}
