using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class Cipher
{
	protected Cipher()
	{
		CipherTags = new List<CipherTag>();
		HintsRequested = new List<HintRequest>();
		AnswerSuggestions = new List<AnswerSuggestion>();
		LLMData = new CipherLLMData();
	}

	[Key]
	public int Id { get; set; }

	[MaxLength(50), MinLength(3)]
	public string Title { get; set; }
	public string? DecryptedText { get; set; }
	public string EncryptedText { get; set; }
	public string MLPrediction { get; set; }
	public CipherLLMData? LLMData { get; set; }
	public CipherType? TypeOfCipher { get; set; }
	public ChallengeType ChallengeType { get; set; }
	public DateTime CreatedAt { get; set; }
	public bool AllowTypeHint { get; set; }
	public bool AllowHint { get; set; }
	public bool AllowSolution { get; set; }
	public ApprovalStatus Status { get; set; }
	public bool IsDeleted { get; set; }
	public DateTime? DeletedAt { get; set; }
	public DateTime? ApprovedAt { get; set; }
	public DateTime? RejectedAt { get; set; }
	public string? RejectionReason { get; set; }
	public bool IsPlaintextValid { get; set; }
	public bool IsLLMRecommended { get; set; }
	public int Points { get; set; }
	[ForeignKey(nameof(CreatedByUser))]
	public string CreatedByUserId { get; set; }
	public ApplicationUser CreatedByUser { get; set; }
	public ICollection<CipherTag> CipherTags { get; set; }
	public ICollection<HintRequest> HintsRequested { get; set; }
	public ICollection<UserSolution> UserSolutions { get; set; }
	public ICollection<AnswerSuggestion> AnswerSuggestions { get; set; }
}

[Owned]
public class CipherLLMData
{
	public string? Reasoning { get; set; }
	public List<string>? Issues { get; set; }
	public string? PredictedType { get; set; }
	public string? Confidence { get; set; }
	public bool? SolutionCorrect { get; set; }
	public bool? IsAppropriate { get; set; }
	public bool? IsSolvable { get; set; }
	public string CachedHint { get; set; } = string.Empty;
	public string CachedSolution { get; set; } = string.Empty;
	public string CachedTypeHint { get; set; } = string.Empty;
}