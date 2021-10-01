using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboScript : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    [SerializeField]
    GameObject posicaoColliderDestino;
    [SerializeField]
    GameObject abrePropriedade;
    [SerializeField]
    GameObject menuControl;
    [SerializeField]
    GameObject panelArquivo;
    [SerializeField]
    GameObject panelPropPeca;
    [SerializeField]
    GameObject panelAjuda;
    [SerializeField]
    GameObject tutorial;

    public static Collider colliderPecas;
    public string concatNumber;

    Vector3 screenPoint, offset, scanPos, startPos;
    GameObject cloneFab;
    float posColliderDestinoX, posColliderDestinoY, posColliderDestinoZ;
    string objName;
    Tutorial tutorialScript;

    void Start()
    {
        scanPos = startPos = gameObject.transform.position;
        tutorialScript = tutorial.GetComponent<Tutorial>();
    }

    void Update()
    {
        scanPos = gameObject.transform.position;
    }

    void OnMouseDown()
    {
        Global.atualizaListaSlot();
        Global.slotName = string.Empty;

        screenPoint = cam.WorldToScreenPoint(scanPos);

        offset = scanPos - cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

        if (screenPoint.y > cam.pixelRect.height / 2) // Pode gerar copia
        {
            CopiaPeca();
        }
        else if (!Input.GetMouseButtonUp(0))
        {
            ConfiguraPropriedadePeca();
        }
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = cam.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    void OnMouseUp()
    {
        if (PodeEncaixar())
        {
            AddCuboTransform();
            ConfiguraColliderDestino();
        }
        else
        {
            AjustaPeca();
        }
    }

    void AjustaPeca()
    {
        if (!tutorialScript.EstaExecutandoTutorial && !CheckPanelIsActive())
        {
            bool podeDestruir = screenPoint.y > cam.pixelRect.height / 2;

            if (podeDestruir && !Global.listaEncaixes.ContainsKey(gameObject.name))
            {
                transform.position = startPos;
                Destroy(cloneFab);
                screenPoint = cam.WorldToScreenPoint(scanPos);
            }
            else
            {
                if (!Global.listaEncaixes.ContainsKey(gameObject.name))
                    transform.position = new Vector3(posColliderDestinoX, posColliderDestinoY, posColliderDestinoZ);

                GameObject newPosition = GameObject.Find(Global.listaEncaixes[gameObject.name]);

                if (newPosition != null)
                {
                    float incX = 0;
                    float incY = 0;

                    AjustaPecaAoSlot(ref incX, ref incY);
                    transform.position = new Vector3(newPosition.transform.position.x + incX, newPosition.transform.position.y - incY, newPosition.transform.position.z);
                }
            }
        }
    }

    bool CheckPanelIsActive()
    {
        return panelArquivo.activeSelf || panelPropPeca.activeSelf || panelAjuda.activeSelf;
    }

    public void ConfiguraColliderDestino()
    {
        float incX = 0;
        float incY = 0;

        if (!tutorialScript.EstaExecutandoTutorial)
            Global.addObject(gameObject);

        SetCubo(ref incX, ref incY);

        // A posi��o x � incrementada para que a pe�a fique no local correto.
        if (!tutorialScript.EstaExecutandoTutorial)
        {
            posColliderDestinoX = posicaoColliderDestino.transform.position.x + incX;
            posColliderDestinoY = posicaoColliderDestino.transform.position.y - incY;
            posColliderDestinoZ = posicaoColliderDestino.transform.position.z;

            transform.position = new Vector3(posColliderDestinoX, posColliderDestinoY, posColliderDestinoZ);
        }
    }

    public void AddCuboTransform()
    {
        if (!tutorialScript.EstaExecutandoTutorial)
        {
            posicaoColliderDestino = GameObject.Find("FormasSlot" + (++DropPeca.countFormas));

            if (Global.cameraAtiva && new PropIluminacaoPadrao().existeIluminacao())
                GameObject.Find("CameraVisInferior").GetComponent<Camera>().cullingMask = 1 << LayerMask.NameToLayer("Formas");

            // Verificar se o Objeto Gr�fico pai est� ativo para demonstrar o cubo.
            string goObjGraficoSlot = GameObject.Find(Global.listaEncaixes[gameObject.name]).transform.parent.name;
            string peca = string.Empty;

            // Verifica pe�a conectada ao slot
            foreach (var pecas in Global.listaEncaixes)
            {
                if (pecas.Value.Equals(goObjGraficoSlot))
                {
                    peca = pecas.Key;
                    break;
                }
            }

            GameObject cuboAmb = GameObject.Find("CuboAmbiente" + GetNumeroSlotObjetoGrafico());
            if (cuboAmb != null)
            {
                cuboAmb.name += DropPeca.countFormas;
            }
            MeshRenderer mr = cuboAmb.GetComponent<MeshRenderer>();

            GameObject cuboVis = GameObject.Find("CuboVis" + GetNumeroSlotObjetoGrafico());
            if (cuboVis != null)
            {
                cuboVis.name += DropPeca.countFormas;
            }

            bool statusCubo = false;
            // Verifica se o Objeto Gr�fico ja foi clicado.
            foreach (var pec in Global.propriedadePecas)
            {
                statusCubo = pec.Key.Equals(peca) ^ Global.propriedadePecas[peca].Ativo;
                break;
            }

            mr.enabled = statusCubo || Global.propriedadePecas.Count.Equals(0);
        }
        else
        {
            GameObject.Find("CuboAmbiente" + GetNumeroSlotObjetoGrafico()).GetComponent<MeshRenderer>().enabled = true;
        }
    }

    void SetCubo(ref float x, ref float y)
    {
        x = 2.2f;
        y = 0.15f;
    }

    void ZerarTransform(GameObject go)
    {
        if (go != null)
        {
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = !go.name.Equals("GameObjectCubo") ? new Vector3(1, 1, 1) : new Vector3(1000, 1000, 1000);
        }
    }

    // firstNameObject = primeiro GameObject pai
    // extensionName   = concatena com o nome do GameObject
    // mainGameObject  = nome do GameObject principal
    void AddGameObjectTree(string firstNameObject, string extensionName, string mainGameObject)
    {
        GameObject goFirst = GameObject.Find(firstNameObject);

        GameObject go = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
        go.name = gameObject.name + extensionName;

        if (goFirst.transform.GetChild(0).name.Contains("CuboAmbiente"))
        {
            go.transform.parent = goFirst.transform;

            GameObject mainGO = GameObject.Find(mainGameObject);

            if (mainGO != null)
                mainGO.transform.parent = go.transform;

            ZerarTransform(go);
            ZerarTransform(mainGO);
        }
        else
        {
            goFirst.transform.GetChild(0).parent = go.transform;
            go.transform.parent = goFirst.transform;
            ZerarTransform(go);
        }

        AtualizaTrasformGameObjectAmb(gameObject.name);
    }

    void CreateGameObjectTree(int numObj, bool tutorial = false)
    {
        // Cria novo 'CuboVisObjectMain'
        GameObject goCubo = Global.cuboVis; //GameObject.Find("CuboVisObjectMain");

        GameObject newGo = Instantiate(goCubo, goCubo.transform.position, goCubo.transform.rotation, goCubo.transform.parent);
        newGo.name = "CuboVisObjectMain" + ((!tutorial) ? numObj.ToString() : "Tutorial");

        newGo.transform.GetChild(0).name += ((!tutorial) ? numObj.ToString() : "Tutorial"); // CuboVisObject
        newGo.transform.GetChild(0).GetChild(0).name += ((!tutorial) ? numObj.ToString() : "Tutorial"); // CuboVis

        // Cria novo 'PosicaoAmb'
        GameObject goPosAmb = Global.posAmb;//GameObject.Find("PosicaoAmb");

        GameObject newGoPos = Instantiate(goPosAmb, goPosAmb.transform.position, goPosAmb.transform.rotation, goPosAmb.transform.parent);
        newGoPos.name = "PosicaoAmb" + ((!tutorial) ? numObj.ToString() : "Tutorial");

        newGoPos.transform.GetChild(0).name += (!tutorial) ? numObj.ToString() : "Tutorial"; // GameObjectAmb
        newGoPos.transform.GetChild(0).GetChild(0).name += (!tutorial) ? numObj.ToString() : "Tutorial"; // CuboAmbiente
    }

    void SetActiveAndRenameGameObject(GameObject goActive, bool tutorial = false)
    {
        foreach (Transform child in goActive.transform)
        {
            if (!child.name.Contains("Slot") && !child.name.Contains("Base")) continue;

            child.gameObject.SetActive(true);

            if (child.name.Contains("Slot"))
            {
                child.name = child.name.Substring(0, child.name.IndexOf("Slot") + 4) + ((!tutorial) ? DropPeca.countObjetosGraficos.ToString() : "Tutorial");
            }
            else
            {
                child.name = child.name.Substring(0, child.name.IndexOf("GO") + 2) + ((!tutorial) ? DropPeca.countObjetosGraficos.ToString() : "Tutorial");

                foreach (Transform _child in child.transform)
                {
                    if (!tutorial)
                    {
                        int value = 0;
                        int.TryParse(_child.name.Substring(_child.name.Length - 1, 1), out value);

                        _child.name = (value > 0) ? _child.name.Substring(0, _child.name.Length - 1) + DropPeca.countObjetosGraficos.ToString() : _child.name + DropPeca.countObjetosGraficos.ToString();
                    }
                    else
                    {
                        _child.name += "Tutorial";
                    }
                }
            }
        }
    }

    public void ConfiguraPropriedadePeca(PropriedadePeca propPeca = null, PropriedadeCamera camProp = null)
    {
        if (Global.listaObjetos != null && Global.listaObjetos.Contains(gameObject))
        {
            Global.gameObjectName = gameObject.name;
            Global.lastPressedButton?.SetActive(false);
            Global.lastPressedButton = abrePropriedade.gameObject;

            CreatePropPeca(propPeca);

            abrePropriedade.GetComponent<PropCuboScript>().Inicializa();
            menuControl.GetComponent<MenuScript>().EnablePanelProp(Global.lastPressedButton.name);
        }
    }

    public void CreatePropPeca(PropriedadePeca propPeca = null)
    {
        if (!Global.propriedadePecas.ContainsKey(gameObject.name))
        {
            PropriedadePeca prPeca;

            if (Global.gameObjectName.Contains(Consts.CUBO))
            {
                prPeca = new CuboPropriedadePeca()
                {
                    NomeCuboAmbiente = "CuboAmbiente" + getNumObjeto(Global.listaEncaixes[gameObject.name]),
                    NomeCuboVis = "CuboVis" + getNumObjeto(Global.listaEncaixes[gameObject.name])
                };
            }
            else if (propPeca == null)
            {
                prPeca = new PropriedadePeca();
            }
            else
            {
                prPeca = propPeca;
            }
            prPeca.Nome = gameObject.name;

            Global.propriedadePecas.Add(gameObject.name, prPeca);
        }
    }

    public bool PodeEncaixar()
    {
        const float VALOR_APROXIMADO = 2;

        float pecaY = transform.position.y;

        foreach (var slot in Global.listaPosicaoSlot)
        {
            //Verifica se o encaixe existe na lista 
            if (slot.Key.Contains(Global.GetSlot(gameObject.name)))
            {
                //Verifica se a pe�a est� pr�xima do encaixe e se o Slot ainda n�o est� na lista de encaixes.
                if ((slot.Value + VALOR_APROXIMADO > pecaY) && (slot.Value - VALOR_APROXIMADO < pecaY) && !Global.listaEncaixes.ContainsValue(slot.Key))
                {
                    if (!Global.listaEncaixes.ContainsKey(gameObject.transform.name) && (GameObject.Find(slot.Key) != null))
                    {
                        Global.listaEncaixes.Add(gameObject.transform.name, slot.Key);
                    }
                    else if (Global.listaEncaixes[gameObject.name] != slot.Key)
                    {
                        ReposicionaSlots(Global.listaEncaixes[gameObject.name], slot.Key);
                        return false;
                    }
                    else
                        return false;

                    return GameObject.Find(slot.Key) != null;
                }
            }
        }

        return false;
    }

    public void ReposicionaSlots(string slotOrigem, string slotDestino)
    {
        //Permite reposicionar o slot somente se for um 'TransformacoesSlot' e os slots estiverem dentro do mesmo Objeto Gr�fico
        if (slotOrigem.Contains("TransformacoesSlot") && getNumObjeto(slotOrigem).Equals(getNumObjeto(slotDestino)))
        {
            float incX = 0;
            float incY = 0;

            AjustaPecaAoSlot(ref incX, ref incY);

            GameObject slot = GameObject.Find(slotDestino);

            transform.position = new Vector3(slot.transform.position.x + incX, slot.transform.position.y - incY, slot.transform.position.z);

            Destroy(GameObject.Find(slotOrigem));

            GameObject t = GameObject.Find(slotDestino);

            int val = 0;
            string countTransformacoes = string.Empty;
            int.TryParse(slotDestino.Substring(slotDestino.IndexOf("_") + 1), out val);

            countTransformacoes = (val > 0) ? (val + 1).ToString() : "1";

            GameObject cloneTrans = Instantiate(t, t.transform.position, t.transform.rotation, t.transform.parent);
            cloneTrans.name = "TransformacoesSlot" + GetNumeroSlotObjetoGrafico() + "_" + countTransformacoes;
            cloneTrans.transform.position = new Vector3(t.transform.position.x, t.transform.position.y - 3f, t.transform.position.z);

            AddTransformacoeSequenciaSlots(cloneTrans.name);

            posicaoColliderDestino = t;

            bool podeReposicionar = false;

            foreach (string _slot in Global.listaSequenciaSlots)
            {
                if (_slot.Contains("TransformacoesSlot" + GetNumeroSlotObjetoGrafico()) && getNumObjeto(_slot).Equals(GetNumeroSlotObjetoGrafico()))
                {
                    if (_slot.Equals(slotOrigem))
                    {
                        podeReposicionar = true;
                        continue;
                    }

                    if (podeReposicionar)
                    {
                        GameObject GO_Slot = GameObject.Find(_slot);
                        GameObject GO_Peca;

                        foreach (var pair in Global.listaEncaixes)
                        {
                            if (pair.Value.Equals(_slot))
                            {
                                GO_Peca = GameObject.Find(pair.Key);
                                GO_Peca.transform.position = new Vector3(GO_Peca.transform.position.x, GO_Peca.transform.position.y + 3f, GO_Peca.transform.position.z);
                                break;
                            }
                        }

                        GO_Slot.transform.position = new Vector3(GO_Slot.transform.position.x, GO_Slot.transform.position.y + 3f, GO_Slot.transform.position.z);
                    }
                }
            }

            Global.listaSequenciaSlots.Remove(slotOrigem);
            Global.listaEncaixes.Remove(gameObject.name);
            Global.listaEncaixes.Add(gameObject.name, slotDestino);

            ReposicionaPosicaoAmbCuboVis();

            AtualizaTrasformGameObjectAmb(gameObject.name);
            //AtualizaTrasformGameObjectAmb(gameObject.name);
        }
    }

    void AddTransformacoeSequenciaSlots(string slot, bool tutorial = false)
    {
        string numObj = GetNumeroSlotObjetoGrafico();
        bool encontrouTransf = false;
        bool isTransf = false;

        for (int i = 0; i < Global.listaSequenciaSlots.Count; i++)
        {
            isTransf = Global.listaSequenciaSlots[i].Contains("TransformacoesSlot" + ((!tutorial) ? numObj : "Tutorial"));

            if (Global.listaSequenciaSlots[i].Contains("TransformacoesSlot" + ((!tutorial) ? numObj : "Tutorial")))
            {
                encontrouTransf = true;
                continue;
            }
            else if (encontrouTransf && !isTransf)
            {
                Global.listaSequenciaSlots.Insert(i, slot);
                break;
            }
        }
    }

    public void CopiaPeca()
    {
        objName = GetNomeObjeto(gameObject.name);

        cloneFab = Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.parent);
        cloneFab.name = objName + DropPeca.countFormas.ToString();
        cloneFab.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
    }

    public string GetNumeroSlotObjetoGrafico()
    {
        GameObject render = GameObject.Find("Render");
        float value = 0f;
        string numSlotObjGrafico = string.Empty;

        foreach (Transform child in render.transform)
        {
            if (child.name.Contains("ObjGraficoSlot")
                && Global.listaPosicaoSlot.ContainsKey(child.name)
                && Global.listaPosicaoSlot.TryGetValue(child.name, out value)
                && (transform.position.y < value)
                && (child.name.Length > "ObjGraficoSlot".Length))
            {
                numSlotObjGrafico = child.name.Substring(child.name.IndexOf("Slot") + 4, 1);
            }
        }

        return numSlotObjGrafico;
    }

    public string getNumObjeto(string objeto)
    {
        int val = 0;
        string numObj = string.Empty;

        if (objeto.Length > objeto.Substring(0, objeto.IndexOf("Slot") + 4).Length)
            int.TryParse(objeto.Substring(objeto.IndexOf("Slot") + 4, 1), out val);

        if (val > 0)
            numObj = val.ToString();

        return numObj;
    }

    void ReposicionaPosicaoAmbCuboVis()
    {
        GameObject goAmb = GameObject.Find("GameObjectAmb" + GetNumeroSlotObjetoGrafico());
        GameObject goTroca = GameObject.Find(gameObject.name + Consts.AMB);

        if (!Equals(goAmb.transform.GetChild(0).name, goTroca.name))
        {
            goTroca.transform.GetChild(0).parent = goTroca.transform.parent;
            goTroca.transform.parent = goAmb.transform;

            goAmb.transform.GetChild(0).parent = goTroca.transform;
        }

    }

    public void AtualizaTrasformGameObjectAmb(string GOname)
    {
        string nomeAmb = "GameObjectAmb";

        if (!tutorialScript.EstaExecutandoTutorial)
            nomeAmb += getNumObjeto(GameObject.Find(Global.listaEncaixes[GOname]).transform.parent.name);

        int cont = 0;
        int breakLoop = 50;

        Transform GoAmb = GameObject.Find(nomeAmb).transform;

        while (cont < breakLoop)
        {
            if (GoAmb.childCount > 0)
                GoAmb = GoAmb.GetChild(0);

            if (GoAmb.name.Contains("CuboAmbiente"))
            {
                Vector3 pos = Vector3.zero;
                Vector3 tam = new Vector3(1, 1, 1);
                Tamanho tamanho;
                Posicao posicao;

                if (Global.propriedadePecas.Count > 0)
                {
                    // Verifica posi��o e tamanho do cubo.
                    foreach (var enc in Global.listaEncaixes)
                    {
                        if (enc.Value.Equals("FormasSlot" + GetNumeroSlotObjetoGrafico())
                            && (Global.propriedadePecas.ContainsKey(enc.Key)))
                        {
                            tamanho = Global.propriedadePecas[enc.Key].Tam;
                            posicao = Global.propriedadePecas[enc.Key].Pos;
                            tam = new Vector3(tamanho.X, tamanho.Y, tamanho.Z);
                            pos = new Vector3(posicao.X * -1, posicao.Y, posicao.Z);
                            break;
                        }
                    }
                }

                GoAmb.localPosition = pos;
                GoAmb.localRotation = Quaternion.Euler(0, 0, 0);
                GoAmb.localScale = tam;
                break;
            }

            // Verifica se o GO foi exclu�do ent�o pega o pr�ximo.
            if (0.Equals(GoAmb.childCount)
                && !GoAmb.name.Contains("CuboAmbiente")
                && (GoAmb.parent.childCount > 1))
            {
                GoAmb = GoAmb.parent.GetChild(1);
            }

            if (GoAmb.name.Contains(Consts.TRANSLADAR))
            {
                if (!Global.propriedadePecas.ContainsKey(GOname))
                    GoAmb.localPosition = Vector3.zero;

                GoAmb.localRotation = Quaternion.Euler(0, 0, 0);
                GoAmb.localScale = new Vector3(1, 1, 1);
            }
            else if (GoAmb.name.Contains(Consts.ROTACIONAR))
            {
                if (!Global.propriedadePecas.ContainsKey(GOname))
                    GoAmb.localRotation = Quaternion.Euler(0, 0, 0);

                GoAmb.localPosition = Vector3.zero;
                GoAmb.localScale = new Vector3(1, 1, 1);
            }
            else if (GoAmb.name.Contains(Consts.ESCALAR))
            {
                if (!Global.propriedadePecas.ContainsKey(GOname))
                    GoAmb.localScale = new Vector3(1, 1, 1);

                GoAmb.localPosition = Vector3.zero;
                GoAmb.localRotation = Quaternion.Euler(0, 0, 0);
            }

            cont++;
        }
    }
}
