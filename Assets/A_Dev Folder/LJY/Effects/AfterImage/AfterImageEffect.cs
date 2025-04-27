using System.Collections;
using UnityEngine;

public class AfterImageEffect : MonoBehaviour
{
    CharacterBase Owner;

    [SerializeField] GameObject AfterImagePrefab; // �ܻ��� ���� ������
    [SerializeField] float AfterImageLifeTime = 0.3f; // �ܻ��� ���� �ð�
    [SerializeField] float SpawnInterval = 0.05f; // �ܻ� ���� ����

    private void Start()
    {
        Owner = GetComponentInParent<CharacterBase>();
    }

    public void StartAfterImageEffect()
    {
        StartCoroutine(CreateAfterIamge());
    }

    IEnumerator CreateAfterIamge()
    {
        yield return new WaitForSeconds(0.05f); // �߻� ����
        //Owner.UsingStateMachine.CurState == Owner._DodgeState | ����� ������ �̿�. ���� �߰� ���� �߻��� ���� ���� ����
        while (Owner.UsingStateMachine.CurState == Owner._DodgeState) 
        {
            GameObject AfterImage = Instantiate(AfterImagePrefab, transform.position, transform.rotation);
            AfterImage.transform.localScale = Owner.transform.lossyScale;
            SpriteRenderer AfterImageSR = AfterImage.GetComponent<SpriteRenderer>();

            // ���� ��������Ʈ ����
            AfterImageSR.sprite = Owner.sr.sprite;
            AfterImageSR.color = new Color(1f, 1f, 1f, 0.5f); // �ܻ��� ���� ����

            // �ܻ� ����
            Destroy(AfterImage, AfterImageLifeTime);

            yield return new WaitForSeconds(SpawnInterval); // ���� ���
        }
    }
}
