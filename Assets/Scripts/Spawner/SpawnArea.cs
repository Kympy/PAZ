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

			// ����Ǯ�� ���� 1���� ���� ��û
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
		// ���(��ġ�̵�, ȸ��, ������)�� �̿��ؼ� ������ġ�� ��Ȯ�� ���� ����Ѵ�.
		Matrix4x4 rMat = Matrix4x4.TRS(transform.position, transform.rotation, size);

		Vector3 randomPos = rMat.MultiplyPoint(new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f)));
		randomPos.y = transform.position.y;
		return randomPos;
	}
#if UNITY_EDITOR
	// ����� �׸� �� ȣ��Ǵ� �Լ�
	private void OnDrawGizmos()
	{
		drawCube(Color.yellow);
	}

	// ����� ������ �Ǿ��� �� ȣ��Ǵ� �Լ�
	private void OnDrawGizmosSelected()
	{
		drawCube(Color.green);
	}

	//
	// ������ �������� ť�� 1�� �׸���
	void drawCube(Color drawColor)
	{
		Gizmos.color = drawColor;
		Vector3 size = transform.lossyScale;
		size.x *= width;
		size.y = 0.1f;
		size.z *= height;

		// ��ġ�� ȸ���� �������� ����� ����� ���ؼ�
		// Gizmos �� �����ϸ� ���� �׸��� Cube�� ����� ����(��ġ�̵�, ȸ��, ������)�� �޴´�.
		Matrix4x4 rMat = Matrix4x4.TRS(transform.position, transform.rotation, size);
		Gizmos.matrix = rMat;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}
#endif // UNITY_EDITOR
}
