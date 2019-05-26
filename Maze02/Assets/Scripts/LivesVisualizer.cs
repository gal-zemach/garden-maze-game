using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public class LivesVisualizer : MonoBehaviour
	{

		[SerializeField] public GameObject LifeGameObject;

		[SerializeField] public int maxLives = 5;

		[SerializeField] public float spaceBetweenLives = 5;


		private GameObject[] allLives = new GameObject[20];

		private int currentLives;

		
		void Awake()
		{
			Vector3 curentPosition = Vector3.zero;

			for (int i = 0; i < maxLives; i++)
			{
				allLives[i] = Instantiate(LifeGameObject, transform);
				allLives[i].SetActive(true);

				allLives[i].transform.localPosition = curentPosition;
				curentPosition += new Vector3(spaceBetweenLives, 0);
			}
			currentLives = maxLives;
			
			LifeGameObject.SetActive(false);
		}

		public int getCurrentLives()
		{
			return currentLives;
		}
		
		public void decreaseLife()
		{
			if (currentLives > 0)
			{
//				allLives[currentLives - 1].GetComponent<Animator>().SetTrigger("removeLife");
				allLives[currentLives - 1].SetActive(false);
			}
			currentLives--;
		}

		private void deactivateObject()
		{
			allLives[currentLives - 1].SetActive(false);
		}

		public void setLives(int numOfLives)
		{
			for (int i = 0; i < maxLives; i++)
			{
				if (i < numOfLives) allLives[i].SetActive(true);
				else allLives[i].SetActive(false);
			}
			currentLives = numOfLives;
		}

	}
}