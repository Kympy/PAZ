using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
	private WaitForSeconds spawnDelay = new WaitForSeconds(1f);

	[SerializeField, Range(0f, 20f)] private float width = 10;
	[SerializeField, Range(0f, 20f)] private float height = 10;

	private int maxCount = 10;
	private int currentCount = 0;

    private void Start()
    {
		StartCoroutine(Spawn());
    }
    private IEnumerator Spawn()
	{
		while (true)
		{
			yield return spawnDelay; // Wait spawn delay

			if (currentCount >= maxCount) // To max count
            {
				continue;
            }

			// 몬스터풀에 몬스터 1개를 생성 요청
			ZombieBase mob = ZombiePool.Instance.GetNormalZombie();
			mob.transform.position = GetRandomPos();// GetRandomPos();
			Debug.Log("This : " + transform.position);
			Debug.Log("Random : " + GetRandomPos());
			mob.InitData(DataManager.Instance.GetZombieData("Normal"));
			currentCount++;
		}
	}

	private Vector3 GetRandomPos()
	{
		Vector3 size = transform.lossyScale;
		size.x *= width;
		size.z *= height;
		// 행렬(위치이동, 회전, 스케일)을 이용해서 랜덤위치의 정확한 값을 계산한다.
		Matrix4x4 rMat = Matrix4x4.TRS(transform.position, transform.rotation, size);

		Vector3 randomPos = rMat.MultiplyPoint(new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f)));
		randomPos.y = transform.position.y;
		return randomPos;
	}
#if UNITY_EDITOR
	// 기즈모를 그릴 때 호출되는 함수
	private void OnDrawGizmos()
	{
		drawCube(Color.yellow);
	}

	// 기즈모가 선택이 되었을 때 호출되는 함수
	private void OnDrawGizmosSelected()
	{
		drawCube(Color.green);
	}

	//
	// 지정된 색상으로 큐브 1개 그리기
	void drawCube(Color drawColor)
	{
		Gizmos.color = drawColor;
		Vector3 size = transform.lossyScale;
		size.x *= width;
		size.y = 0.1f;
		size.z *= height;

		// 위치와 회전과 스케일이 적용된 행렬을 구해서
		// Gizmos 에 적용하면 이후 그리는 Cube는 행렬의 영향(위치이동, 회전, 스케일)을 받는다.
		Matrix4x4 rMat = Matrix4x4.TRS(transform.position, transform.rotation, size);
		Gizmos.matrix = rMat;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}
#endif // UNITY_EDITOR
}
