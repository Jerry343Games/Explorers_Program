using UnityEngine;
using UnityEditor;

public class ResetAxis : ScriptableObject
{
    //����ģ�͵�����Ϊ����
    [MenuItem("Tools/MyTool/ResetAxis")]
    static void ResetAxisssssss()
    {
        //��ȡѡ�е�����
        GameObject target = Selection.activeGameObject;
        string dialogTitle = "Tools/MyTool/ResetAxis";

        if (target == null)
        {
            EditorUtility.DisplayDialog(dialogTitle, "û��ѡ����Ҫ�������ĵ�����!!!", "ȷ��");
            return;
        }

        //��ȡĿ������������������Ⱦ
        MeshRenderer[] meshRenderers = target.GetComponentsInChildren<MeshRenderer>(true);
        if (meshRenderers.Length == 0)
        {
            EditorUtility.DisplayDialog(dialogTitle, "ѡ�е����岻����Чģ������!!!", "ȷ��");
            return;
        }
        //�����е�������Ⱦ�ı߽���кϲ�
        Bounds centerBounds = meshRenderers[0].bounds;
        for (int i = 1; i < meshRenderers.Length; i++)
        {
            centerBounds.Encapsulate(meshRenderers[i].bounds);
        }
        //����Ŀ��ĸ�����
        Transform targetParent = new GameObject(target.name + "-Parent").transform;

        //���Ŀ��ԭ�����и�����,�򽫴���Ŀ�길����ĸ�������Ϊԭ������;
        Transform originalParent = target.transform.parent;
        if (originalParent != null)
        {
            targetParent.SetParent(originalParent);
        }
        //����Ŀ�길�����λ��Ϊ�ϲ����������Ⱦ�߽�����
        targetParent.position = centerBounds.center;
        //����Ŀ������ĸ�����
        target.transform.parent = targetParent;

        Selection.activeGameObject = targetParent.gameObject;
        EditorUtility.DisplayDialog(dialogTitle, "����ģ��������������!", "ȷ��");
    }
}
