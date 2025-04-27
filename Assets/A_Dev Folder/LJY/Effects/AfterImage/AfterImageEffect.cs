using System.Collections;
using UnityEngine;

public class AfterImageEffect : MonoBehaviour
{
    CharacterBase Owner;

    [SerializeField] GameObject AfterImagePrefab; // 잔상을 위한 프리팹
    [SerializeField] float AfterImageLifeTime = 0.3f; // 잔상의 지속 시간
    [SerializeField] float SpawnInterval = 0.05f; // 잔상 생성 간격

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
        yield return new WaitForSeconds(0.05f); // 발생 유예
        //Owner.UsingStateMachine.CurState == Owner._DodgeState | 현재는 닷지만 이용. 이후 추가 조건 발생시 전용 변수 생성
        while (Owner.UsingStateMachine.CurState == Owner._DodgeState) 
        {
            GameObject AfterImage = Instantiate(AfterImagePrefab, transform.position, transform.rotation);
            AfterImage.transform.localScale = Owner.transform.lossyScale;
            SpriteRenderer AfterImageSR = AfterImage.GetComponent<SpriteRenderer>();

            // 현재 스프라이트 복사
            AfterImageSR.sprite = Owner.sr.sprite;
            AfterImageSR.color = new Color(1f, 1f, 1f, 0.5f); // 잔상의 투명도 설정

            // 잔상 삭제
            Destroy(AfterImage, AfterImageLifeTime);

            yield return new WaitForSeconds(SpawnInterval); // 간격 대기
        }
    }
}
