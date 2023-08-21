using System.Collections.Generic;
using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.Factories;
using GrazerCore.GameElements.EnemyGroup;
using UnityEditor;
using GrazerCore.Interfaces;

namespace GrazerCore.Editor
{
    [CustomEditor(typeof(EnemyTeam))]
    public class EnemyTeamEditor : UnityEditor.Editor
    {
        EnemyTeam enemyTeam;
        EnemyTeam nextTeam;
        float waitTime = 0;
        List<Enemy> summonEnemys = new List<Enemy>();
        Coroutine nextTeamCallRoutine = null;
        Coroutine teamSummonRoutine = null;
        Coroutine nextTeamSummonRoutine = null;

        #region Editor GUI 
        private float bigSpace = 50;
        private string msgSaveTitle = "Save";
        private string msgSaveContent = "Overwrite current data?";
        private string msgSaveConfirm = "Confirm";
        private string msgSaveCancel = "Cancel";
        #endregion

        private void OnEnable()
        {
            enemyTeam = (EnemyTeam)target;
            enemyTeam.OnMemberCreate.AddListener((Enemy enemy) =>
            {
                summonEnemys.Add(enemy);
            });
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(bigSpace);
            if (!Application.isPlaying)
            {
                GUILayout.Label("Editor Part");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Load"))
                {
                    LoadEnemyTeam();
                }
                if (GUILayout.Button("Save"))
                {
                    SaveTeam();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Test Part");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Summon"))
                {
                    StopAllSummon();
                    TestSummonForCurrentTeam();
                    if (nextTeam != null)
                    {
                        DelayCallNextTeam();
                    }
                }
                if (GUILayout.Button("StopSummon"))
                {
                    StopAllSummon();
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Label("Next team");
                nextTeam = (EnemyTeam)EditorGUILayout.ObjectField(nextTeam, typeof(EnemyTeam), true);
                waitTime = EditorGUILayout.FloatField("DelayTime",waitTime);
            }
        }

        private void LoadEnemyTeam()
        {
            var teamDatas = enemyTeam.MemberDatas;
            var memberCount = teamDatas.Length;
            for (int index = 0; index < memberCount; ++index)
            {
                var memberData = teamDatas[index];
                var go = PrefabUtility.InstantiatePrefab(memberData.EnemyPrefab);
                var enemy = (Enemy)go;
                enemy.transform.SetParent(enemyTeam.transform);
                enemy.transform.localPosition = memberData.SetPosition;
                var enemySpawnHelper = enemy.gameObject.AddComponent<EnemySpawnHelper>();
                enemySpawnHelper.m_TargetObject = enemy;
                enemySpawnHelper.m_DelayTime = memberData.DelaySpawnTime;
                enemySpawnHelper.m_LogicData = memberData.LogicData;
                enemySpawnHelper.SetLogicData(enemySpawnHelper.m_LogicData);
            }
        }

        private void SaveTeam()
        {
            if (EditorUtility.DisplayDialog(msgSaveTitle, msgSaveContent, msgSaveConfirm, msgSaveCancel))
            {
                var childCount = enemyTeam.transform.childCount;
                List<EnemyTeamMemberData> newData = new List<EnemyTeamMemberData>();
                for (int index = 0; index < childCount; ++index)
                {
                    var child = enemyTeam.transform.GetChild(index);
                    
                    if (PrefabUtility.IsPartOfAnyPrefab(child))
                    {
                        var enemy = child.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            var enemyPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(enemy);
                            var setPos = enemy.transform.localPosition;
                            var spawnHelper = enemy.gameObject.GetComponent<EnemySpawnHelper>();
                            string logicData = "";
                            float delayTime = 0;
                            if (spawnHelper != null)
                            {
                                logicData = spawnHelper.GetLogicData();
                                delayTime = spawnHelper.m_DelayTime;
                            }
                            var newMemberData = new EnemyTeamMemberData(enemyPrefab, setPos, delayTime, logicData);
                            newData.Add(newMemberData);
                        }
                    }
                }

                enemyTeam.MemberDatas = newData.ToArray();
                for (int index = childCount - 1; index >= 0; --index)
                {
                    var child = enemyTeam.transform.GetChild(index);
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        private void StopAllSummon()
        {
            var summonEnemyCount = summonEnemys.Count;
            for (int index = 0; index < summonEnemyCount; ++index)
            {
                var enemy = summonEnemys[index];
                enemy.Recycle();
            }
            summonEnemys.Clear();

            StopSummonForCurrentTeam();
            if (nextTeam != null)
            {
                StopSummonForNextTeam();
                if (nextTeamCallRoutine != null)
                {
                    enemyTeam.StopCoroutine(nextTeamCallRoutine);
                }
            }
        }

        private void DelayCallNextTeam()
        {
            nextTeam.OnMemberCreate.RemoveAllListeners();
            nextTeam.OnMemberCreate.AddListener((Enemy enemy) =>
            {
                summonEnemys.Add(enemy);
            });
            nextTeamCallRoutine = enemyTeam.StartCoroutine(DelayCall());
            Debug.Log("Ready call next team "+Time.time);
        }

        private System.Collections.IEnumerator DelayCall()
        {
            yield return new WaitForSeconds(waitTime);
            Debug.Log("Next team start summon" + Time.time);
            TestSummonForNextTeam();
        }

        #region For test Summon
        private void TestSummonForCurrentTeam()
        {
            var memberCount = enemyTeam.MemberDatas.Length;
            SummonForCurrentTeam(memberCount, 0);
        }

        private void SummonForCurrentTeam(int memberCount, int summonIndex)
        {
            if (summonIndex >= memberCount)
            {
                return;
            }
            StopSummonForCurrentTeam();
            teamSummonRoutine = enemyTeam.StartCoroutine(SummoningForCurrentTeam(memberCount, summonIndex));
        }

        private System.Collections.IEnumerator SummoningForCurrentTeam(int memberCount, int summonIndex)
        {
            var memberData = enemyTeam.MemberDatas[summonIndex];
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
            enemyTeam.OnMemberCreate?.Invoke(getEnemy);
            summonIndex++;
            SummonForCurrentTeam(memberCount, summonIndex);
        }

        private void StopSummonForCurrentTeam()
        {
            if (teamSummonRoutine != null)
            {
                enemyTeam.StopCoroutine(teamSummonRoutine);
                teamSummonRoutine = null;
            } else
            {
                return;
            }
        }
        #endregion

        #region For test Summon next team
        private void TestSummonForNextTeam()
        {
            var memberCount = nextTeam.MemberDatas.Length;
            SummonForNextTeam(memberCount, 0);
        }

        private void SummonForNextTeam(int memberCount, int summonIndex)
        {
            if (summonIndex >= memberCount)
            {
                return;
            }
            StopSummonForNextTeam();
            nextTeamSummonRoutine = nextTeam.StartCoroutine(SummoningForNextTeam(memberCount, summonIndex));
        }

        private System.Collections.IEnumerator SummoningForNextTeam(int memberCount, int summonIndex)
        {
            var memberData = nextTeam.MemberDatas[summonIndex];
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
            nextTeam.OnMemberCreate?.Invoke(getEnemy);
            summonIndex++;
            SummonForNextTeam(memberCount, summonIndex);
        }

        private void StopSummonForNextTeam()
        {
            if (nextTeamSummonRoutine != null)
            {
                nextTeam.StopCoroutine(nextTeamSummonRoutine);
                nextTeamSummonRoutine = null;
            }
            else
            {
                return;
            }
        }
        #endregion
    }
}
