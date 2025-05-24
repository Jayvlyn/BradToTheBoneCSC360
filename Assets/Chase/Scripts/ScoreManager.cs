using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
	[SerializeField] private TMP_Text scoreText;
	[SerializeField] private string textBefore = "Score: ";

	private IScoreMediator mediator;

	private float buildupScore = 0f;
	private float elapsedTime = 0f;
	private readonly float countUpTime = 7.775f;

	private void Awake()
	{
		mediator = new ScoreMediator();
		mediator.LoadBestScore();
		mediator.RegisterOnScoreChanged(OnScoreChanged);
	}

	private void Start()
	{
		UpdateScoreText();
	}

	private void OnScoreChanged(int newScore)
	{
		buildupScore = 0f;
		elapsedTime = 0f;
		UpdateScoreText();
	}

	private void Update()
	{
		if (mediator.IsGameOverScene())
		{
			int targetScore = mediator.GetBestScore();

			if (targetScore - buildupScore < 100)
			{
				buildupScore = targetScore;
			}
			else
			{
				buildupScore = Mathf.Lerp(0, targetScore, elapsedTime / countUpTime);
				elapsedTime += Time.deltaTime;
			}

			scoreText.text = textBefore + (int)buildupScore;
		}
		else
		{
			UpdateScoreText();
		}
	}

	private void UpdateScoreText()
	{
		int displayScore = mediator.IsGameOverScene() ? (int)buildupScore : mediator.GetCurrentScore();
		scoreText.text = textBefore + displayScore;
	}

	public void AddScore(int amount)
	{
		int newScore = mediator.GetCurrentScore() + amount;
		mediator.UpdateCurrentScore(newScore);
	}

	public void SaveBestScore()
	{
		mediator.SaveBestScore();
	}
}
