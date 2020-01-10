using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour, IOnSceneLoad
{
	[Header("UI Properties")]
	[SerializeField] private int _maxPlayerHP = 5;
	[Header("UI References")]
	[SerializeField] private Image _hpImage = null;

	private Animator _anim;

	private float GetHealthPercentage(float current) => current / (float)_maxPlayerHP;

	// Start is called before the first frame update
	void Start()
    {
		_anim = GetComponent<Animator>();
		LoadSave.AddActionToScenesLoad(OnSceneLoad);
		//AssignListener();
		ResetPlayerUI();
	}

	private void UpdateHp(int newHp)
	{
		if(newHp > 0)
			_anim.SetTrigger("Hit");
		else
			_anim.SetTrigger("Death");

		SetImageFill(_hpImage, GetHealthPercentage((float)newHp));
	}

	private void SetImageFill(Image targetImg, float fill)
	{
		targetImg.fillAmount = Mathf.Clamp01(fill);
	}

	private void AssignListener()
	{
		Player.Instance?.AddUIHPListener(UpdateHp);
	}

	private void ResetPlayerUI()
	{
		AssignListener();
		if (Player.Instance != null)
			SetImageFill(_hpImage, GetHealthPercentage((float)Player.Instance.HP));
		_anim.SetTrigger("Reset");
	}

	public void HideUI()
	{
		_anim.SetTrigger("Hide");
	}

	public void UnHideUI()
	{
		_anim.SetTrigger("Reset");
	}

	public void OnSceneLoad(Scene scene, LoadSceneMode mode)
	{
		//ResetPlayerUI();
	}
}
