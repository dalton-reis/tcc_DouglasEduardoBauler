﻿using System;
using UnityEngine;

public class LixeiraScript : MonoBehaviour
{
    [SerializeField]
    GameObject tutorial;
    [SerializeField]
    GameObject render;
    [SerializeField]
    GameObject propCamera;
    [SerializeField]
    GameObject menuControl;
    [SerializeField]
    GameObject fabricaPecas;
    [SerializeField]
    GameObject posicaoAmb;
    [SerializeField]
    GameObject posicaoVis;

    GameObject objDrop;
    Controller controller;

    void FixedUpdate()
    {
        if (Input.GetMouseButtonUp(0) && objDrop != null)
        {
            RemovePeca();
            objDrop = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        objDrop = other.gameObject;

        if (objDrop.name.Contains(Consts.ILUMINACAO))
        {
            controller = objDrop.GetComponent<Controller>();
        }
    }

    void RemovePeca()
    {
        EnabledColliderPecas(false);
        try
        {
            if (objDrop.name.Contains(Consts.OBJETOGRAFICO))
            {
                RemoveObjetoGrafico();
            }
            else
            {
                if (objDrop.name.Contains(Consts.CUBO) || objDrop.name.Contains(Consts.POLIGONO) || objDrop.name.Contains(Consts.SPLINE))
                {
                    RemoveForma();
                }
                else
                {
                    if (objDrop.name.Contains(Consts.ROTACIONAR) || objDrop.name.Contains(Consts.ESCALAR) || objDrop.name.Contains(Consts.TRANSLADAR))
                    {
                        RemoveTransformacao();
                    }
                    else
                    {
                        if (objDrop.name.Contains(Consts.ITERACAO))
                        {
                            RemoveIteracao();
                        }
                        else if (objDrop.name.Contains(Consts.ILUMINACAO))
                        {
                            RemoveIluminacao();
                        }
                    }
                }
            }

            menuControl.GetComponent<MenuScript>().EnablePanelFabPecas();
            ReactiveColliderPeca();
        }
        finally
        {
            EnabledColliderPecas(true);
        }
    }

    void RemoveIluminacao()
    {
        throw new NotImplementedException();
    }

    void RemoveIteracao()
    {
        throw new NotImplementedException();
    }

    void EnabledColliderPecas(bool status)
    {
        foreach (var item in Global.listaEncaixes)
        {
            GameObject.Find(item.Key).GetComponent<BoxCollider>().enabled = status;
        }
    }

    void RemoveObjetoGrafico()
    {
        GameObject forma = GetForma();
        string numFormaSlot = Util_VisEdu.GetNumSlot(objDrop.transform.parent.name);
        Transform slot;

        foreach (Transform child in posicaoAmb.transform)
        {
            if (child.name.Contains(Consts.CUBO_AMB_OBJ + numFormaSlot)
                || child.name.Contains(Consts.POLIGONO_AMB_OBJ + numFormaSlot)
                || child.name.Contains(Consts.SPLINE_AMB_OBJ + numFormaSlot))
            {
                Destroy(child.gameObject);
            }
        }

        foreach (Transform child in posicaoVis.transform)
        {
            if (child.name.Contains(Consts.CUBO_AMB_OBJ + numFormaSlot)
                || child.name.Contains(Consts.POLIGONO_AMB_OBJ + numFormaSlot)
                || child.name.Contains(Consts.SPLINE_AMB_OBJ + numFormaSlot))
            {
                Destroy(child.gameObject);
            }
        }

        foreach (Transform child in render.transform)
        {
            if (child.name.Contains(Consts.SLOT_TRANSF + numFormaSlot))
            {
                slot = child.transform.GetChild(0);

                foreach (Transform _slot in slot.transform)
                {
                    if (_slot.name.Contains(Consts.ROTACIONAR)
                        || _slot.name.Contains(Consts.TRANSLADAR)
                        || _slot.name.Contains(Consts.ESCALAR))
                    {
                        Global.listaEncaixes.Remove(_slot.name);
                        Global.propriedadePecas.Remove(_slot.name);
                    }
                }
                Destroy(child.gameObject);
            }
            else if (child.name.Contains(Consts.SLOT_FORMA + numFormaSlot))
            {
                slot = child.transform.GetChild(0);

                foreach (Transform _slot in slot.transform)
                {
                    if (_slot.name.Contains(Consts.CUBO)
                        || _slot.name.Contains(Consts.POLIGONO)
                        || _slot.name.Contains(Consts.SPLINE))
                    {
                        Global.listaEncaixes.Remove(_slot.name);
                        Global.propriedadePecas.Remove(_slot.name);
                    }
                }
                Destroy(child.gameObject);
            }
            else if (child.name.Contains(Consts.SLOT_ILUMINACAO + numFormaSlot))
            {
                slot = child.transform.GetChild(0);

                foreach (Transform _slot in slot.transform)
                {
                    if (_slot.name.Contains(Consts.ILUMINACAO))
                    {
                        // Rever
                        Global.listaEncaixes.Remove(_slot.name);
                        Global.propriedadePecas.Remove(_slot.name);
                    }
                }
                Destroy(child.gameObject);
            }
            else if (child.name.Contains(Consts.SLOT_OBJ_GRAFICO + numFormaSlot))
            {
                slot = child.transform.GetChild(0);

                foreach (Transform _slot in slot.transform)
                {
                    if (_slot.name.Contains(Consts.OBJETOGRAFICO))
                    {
                        Global.listaEncaixes.Remove(_slot.name);
                        Global.propriedadePecas.Remove(_slot.name);
                    }
                }
                Destroy(child.gameObject);
            }
        }
    }

    void RemoveTransformacao()
    {
        string numTransfSlot = Util_VisEdu.GetNumSlot(objDrop.transform.parent.name, true);
        Transform slot;

        foreach (Transform child in posicaoAmb.transform)
        {
            if (child.childCount > 0)
            {
                if (child.GetChild(0).name.Contains(objDrop.name))
                {
                    if (child.GetChild(0).childCount > 0)
                    {
                        child.GetChild(0).GetChild(0).parent = child.GetChild(0).parent;
                    }
                    Destroy(child.GetChild(0).gameObject);
                    break;
                }
                else
                {
                    if (child.GetChild(0).childCount > 0)
                    {
                        if (child.GetChild(0).GetChild(0).name.Contains(objDrop.name))
                        {
                            if (child.GetChild(0).GetChild(0).childCount > 0)
                            {
                                child.GetChild(0).GetChild(0).GetChild(0).parent = child.GetChild(0).GetChild(0).parent;
                            }
                            Destroy(child.GetChild(0).GetChild(0).gameObject);
                            break;
                        }
                        else
                        {
                            if (child.GetChild(0).GetChild(0).childCount > 0 && child.GetChild(0).GetChild(0).GetChild(0).name.Contains(objDrop.name))
                            {
                                if (child.GetChild(0).GetChild(0).GetChild(0).childCount > 0)
                                {
                                    child.GetChild(0).GetChild(0).GetChild(0).GetChild(0).parent = child.GetChild(0).GetChild(0).GetChild(0).parent;
                                }
                                Destroy(child.GetChild(0).GetChild(0).GetChild(0).gameObject);
                                break;
                            }
                        }
                    }
                }
            }
        }

        foreach (Transform child in posicaoVis.transform)
        {
            if (child.childCount > 0)
            {
                if (child.GetChild(0).name.Contains(objDrop.name))
                {
                    if (child.GetChild(0).childCount > 0)
                    {
                        child.GetChild(0).GetChild(0).parent = child.GetChild(0).parent;
                    }
                    Destroy(child.GetChild(0).gameObject);
                    break;
                }
                else
                {
                    if (child.GetChild(0).childCount > 0)
                    {
                        if (child.GetChild(0).GetChild(0).name.Contains(objDrop.name))
                        {
                            if (child.GetChild(0).GetChild(0).childCount > 0)
                            {
                                child.GetChild(0).GetChild(0).GetChild(0).parent = child.GetChild(0).GetChild(0).parent;
                            }
                            Destroy(child.GetChild(0).GetChild(0).gameObject);
                            break;
                        }
                        else
                        {
                            if (child.GetChild(0).GetChild(0).childCount > 0 && child.GetChild(0).GetChild(0).GetChild(0).name.Contains(objDrop.name))
                            {
                                if (child.GetChild(0).GetChild(0).GetChild(0).childCount > 0)
                                {
                                    child.GetChild(0).GetChild(0).GetChild(0).GetChild(0).parent = child.GetChild(0).GetChild(0).GetChild(0).parent;
                                }
                                Destroy(child.GetChild(0).GetChild(0).GetChild(0).gameObject);
                                break;
                            }
                        }
                    }
                }
            }
        }

        foreach (Transform child in render.transform)
        {
            if (child.name.Contains(Consts.SLOT_TRANSF + numTransfSlot))
            {
                slot = child.transform.GetChild(0);

                foreach (Transform _slot in slot.transform)
                {
                    if (_slot.name.Contains(objDrop.name))
                    {
                        Global.listaEncaixes.Remove(_slot.name);
                        Global.propriedadePecas.Remove(_slot.name);
                        break;
                    }
                }
                Destroy(child.gameObject);
                break;
            }
        }
    }

    void RemoveForma()
    {
        GameObject forma = GetForma();
        string numFormaSlot = Util_VisEdu.GetNumSlot(forma.transform.parent.name);
        Transform slot;

        foreach (Transform child in posicaoAmb.transform)
        {
            if (child.name.Contains(Consts.CUBO_AMB_OBJ + numFormaSlot)
                || child.name.Contains(Consts.POLIGONO_AMB_OBJ + numFormaSlot)
                || child.name.Contains(Consts.SPLINE_AMB_OBJ + numFormaSlot))
            {
                Destroy(child.gameObject);
                break;
            }
        }

        foreach (Transform child in posicaoVis.transform)
        {
            if (child.name.Contains(Consts.CUBO_AMB_OBJ + numFormaSlot)
                || child.name.Contains(Consts.POLIGONO_AMB_OBJ + numFormaSlot)
                || child.name.Contains(Consts.SPLINE_AMB_OBJ + numFormaSlot))
            {
                Destroy(child.gameObject);
                break;
            }
        }

        foreach (Transform child in render.transform)
        {
            if (child.name.Contains(Consts.SLOT_TRANSF + numFormaSlot + "_"))
            {
                slot = child.transform.GetChild(0);

                foreach (Transform _slot in slot.transform)
                {
                    if (_slot.name.Contains(Consts.ROTACIONAR)
                        || _slot.name.Contains(Consts.TRANSLADAR)
                        || _slot.name.Contains(Consts.ESCALAR))
                    {
                        Global.listaEncaixes.Remove(_slot.name);
                        Global.propriedadePecas.Remove(_slot.name);
                    }
                }
                Destroy(child.gameObject);
            }
            else if (child.name.Contains(Consts.SLOT_FORMA + numFormaSlot))
            {
                slot = child.transform.GetChild(0);

                foreach (Transform _slot in slot.transform)
                {
                    if (_slot.name.Contains(Consts.CUBO)
                        || _slot.name.Contains(Consts.POLIGONO)
                        || _slot.name.Contains(Consts.SPLINE))
                    {
                        Global.listaEncaixes.Remove(_slot.name);
                        Global.propriedadePecas.Remove(_slot.name);
                        Destroy(_slot.gameObject);
                    }
                }
            }
            else if (child.name.Contains(Consts.SLOT_ILUMINACAO + numFormaSlot))
            {
                slot = child.transform.GetChild(0);

                foreach (Transform _slot in slot.transform)
                {
                    if (_slot.name.Contains(Consts.ILUMINACAO))
                    {
                        // Rever
                        Global.listaEncaixes.Remove(_slot.name);
                        Global.propriedadePecas.Remove(_slot.name);
                        //Destroy(_slot.gameObject);
                    }
                }
            }
        }

        if (forma.transform.parent.name.Contains(Consts.FORMA_SLOT))
        {
            Rigidbody rigiBody = forma.transform.parent.gameObject.AddComponent<Rigidbody>();
            rigiBody.useGravity = false;
            rigiBody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    GameObject GetForma()
    {
        string numForma = Util_VisEdu.GetNumSlot(objDrop.transform.parent.name);

        GameObject forma = GameObject.Find(Consts.CUBO + numForma);
        if (forma != null)
        {
            return forma;
        }

        forma = GameObject.Find(Consts.POLIGONO + numForma);
        if (forma != null)
        {
            return forma;
        }

        forma = GameObject.Find(Consts.SPLINE + numForma);
        if (forma != null)
        {
            return forma;
        }

        return null;
    }

    void DestroyIluminacao(string key)
    {
        PropIluminacaoPadrao luz = new PropIluminacaoPadrao();

        if (key.Contains(Consts.ILUMINACAO))
        {
            Global.propriedadeIluminacao.Remove(key);

            if (key.Length > "Iluminacao".Length)
            {
                Destroy(GameObject.Find("LightObjects" + key));
            }
            else if (Global.propriedadePecas.ContainsKey(key))
            {
                luz.AtivaIluminacao(luz.GetTipoLuzPorExtenso(Global.propriedadePecas[key].TipoLuz) + key, false);
            }
        }
    }

    void configuraIluminacao(string sinal)
    {
        float valorInc = 0;
        string iluminacao = "Iluminacao";

        valorInc = sinal.Equals("+") ? 3f : -3f;

        GameObject goObj = GameObject.Find("ObjGraficoSlot" + controller.GetComponent<Controller>().concatNumber);

        foreach (Transform child in goObj.transform)
        {
            if (child.name.Contains(Consts.ILUMINACAO_SLOT))
            {
                foreach (var peca in Global.listaEncaixes)
                {
                    if (child.name.Equals(peca.Value))
                        iluminacao = peca.Key;
                }
            }
        }

        GameObject ilumunacao = GameObject.Find(Consts.ILUMINACAO_SLOT + controller.GetComponent<Controller>().concatNumber);
        Vector3 pos = ilumunacao.transform.position;
        pos.y += valorInc;
        ilumunacao.transform.position = pos;

        // Se a peça "Iluminação já foi selecionada, será devidamente reposicionada"        
        GameObject IlumPeca = GameObject.Find(iluminacao);

        if (!tutorial.GetComponent<Tutorial>().EstaExecutandoTutorial
            //&& Global.listaObjetos.Contains(IlumPeca)
            )
        {
            IlumPeca.transform.position = new Vector3(IlumPeca.transform.position.x, pos.y, IlumPeca.transform.position.z);
        }
    }

    void ReactiveColliderPeca()
    {
        BoxCollider collider;

        foreach (Transform child in fabricaPecas.transform)
        {
            collider = child.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }
}
