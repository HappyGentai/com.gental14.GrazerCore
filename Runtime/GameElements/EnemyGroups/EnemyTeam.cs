using UnityEngine;
using GrazerCore.Factories;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using GrazerCore.Interfaces;

namespace GrazerCore.GameElements.EnemyGroup
{
    public class EnemyTeam : MonoBehaviour
    {
        [SerializeField]
        private EnemyTeamMemberData[] m_MemberDatas = null;
        public EnemyTeamMemberData[] MemberDatas
        {
            get { return m_MemberDatas; }
            set
            {
                m_MemberDatas = value;
            }
        }
        [SerializeField]
        private bool m_SummonWhenStart = false;
        private int memberLiveCount = 0;
        public int MemberLiveCount
        {
            get { return memberLiveCount; }
        }
        private UnityEvent<Enemy> onMemberCreate = new UnityEvent<Enemy>();
        public UnityEvent<Enemy> OnMemberCreate
        {
            get { return onMemberCreate; }
        }
        private UnityEvent<EnemyTeam> onAllMemberGone = new UnityEvent<EnemyTeam>();
        public UnityEvent<EnemyTeam> OnAllMemberGone
        {
            get { return onAllMemberGone; }
        }

        private UnityEvent onSummonDone = new UnityEvent();
        public UnityEvent OnSummonDone
        {
            get { return onSummonDone; }
        }

        private int memberCount = 0;
        private int summonIndex = 0;
        private Coroutine summonRoutine = null;
        private bool _IsWorking = false;
        public bool IsWorking
        {
            get { return _IsWorking; }
        }

        private List<Enemy> teamMember = new List<Enemy>();

        private void Start()
        {
            if (m_SummonWhenStart)
            {
                SummonMember();
            }
        }

        public void SummonMember()
        {
            _IsWorking = true;
            //  Clear old member event(if have)
            var oldMemberCount = teamMember.Count;
            for (int index = 0; index < oldMemberCount; ++index)
            {
                var oldMember = teamMember[index];
                if (oldMember != null)
                {
                    oldMember.OnRecycle.RemoveListener(OnTeamMemberRelease);
                }
            }
            teamMember.Clear();

            memberCount = m_MemberDatas.Length;
            memberLiveCount = memberCount;
            summonIndex = 0;
            Summon();
        }

        private void StopSummon()
        {
            if (summonRoutine != null)
            {
                StopCoroutine(summonRoutine);
            }
        }

        public void Close()
        {
            StopSummon();
            _IsWorking = false;
        }

        private void Summon()
        {
            if (summonIndex >= memberCount)
            {
                return;
            }
            var memberData = m_MemberDatas[summonIndex];
            StopSummon();
            summonRoutine = StartCoroutine(Summoning(memberData));
            summonIndex++;
        }

        private void OnTeamMemberRelease(Enemy enemy)
        {
            enemy.OnRecycle.RemoveListener(OnTeamMemberRelease);
            memberLiveCount--;
            if (memberLiveCount == 0)
            {
                //  Member all dead or be recycle
                OnAllMemberGone.Invoke(this);
                OnSummonDone?.Invoke();
            }
        }

        private IEnumerator Summoning(EnemyTeamMemberData memberData)
        {
            var delayTime = new WaitForSeconds(memberData.DelaySpawnTime);
            yield return delayTime;
            var getEnemy = EnemyFactory.GetEnemy(memberData.EnemyPrefab);
            getEnemy.MoveTarget.localPosition = memberData.SetPosition;
            //  Check have logic or not
            if (getEnemy is ILogicDataSetable iDataSetable)
            {
                iDataSetable.SetLogicData(memberData.LogicData);
            }
            getEnemy.StartAction();
            getEnemy.CanShoot(false);
            getEnemy.SetInvincible(true);
            getEnemy.OnRecycle.AddListener(OnTeamMemberRelease);
            teamMember.Add(getEnemy);
            OnMemberCreate?.Invoke(getEnemy);
            Summon();
        }
    }
}
