using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[Header("UI Properties")]
	[SerializeField] private int _maxPlayerHP = 5;
	[Header("UI References")]
	[SerializeField] private Image _hpImage = null;

	private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
		Player.Instance?.AddUIHPListener(UpdateHp);
		_anim = GetComponent<Animator>();
	}

	private void UpdateHp(int newHp)
	{
		float newFill = (float)newHp / (float)_maxPlayerHP;
		if(newHp > 0)
			_anim.SetTrigger("Hit");
		else
			_anim.SetTrigger("Death");

		SetImageFill(_hpImage, newFill);
	}

	private void SetImageFill(Image targetImg, float fill)
	{
		targetImg.fillAmount = Mathf.Clamp01(fill);
	}
}
