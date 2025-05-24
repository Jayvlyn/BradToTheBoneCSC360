using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScoreMediator
{
	int GetCurrentScore();
	int GetBestScore();
	void UpdateCurrentScore(int newScore);
	void SaveBestScore();
	void LoadBestScore();
	bool IsGameOverScene();
	void RegisterOnScoreChanged(System.Action<int> callback);
}

public class ScoreMediator : IScoreMediator
{
	private int currentScore = 0;
	private int bestScore = 0;

	private event Action<int> OnScoreChanged;

	public int GetCurrentScore() => currentScore;
	public int GetBestScore() => bestScore;

	public void UpdateCurrentScore(int newScore)
	{
		currentScore = newScore;
		OnScoreChanged?.Invoke(currentScore);
	}

	public void SaveBestScore()
	{
		if (currentScore > bestScore)
		{
			bestScore = currentScore;
			PlayerPrefs.SetInt("BestScore", bestScore);
			PlayerPrefs.Save();
		}
	}

	public void LoadBestScore()
	{
		bestScore = PlayerPrefs.GetInt("BestScore", 0);
	}

	public bool IsGameOverScene()
	{
		return GameManager.GetActiveSceneName().Equals("GameOver");
	}

	public void RegisterOnScoreChanged(Action<int> callback)
	{
		OnScoreChanged += callback;
	}
}

