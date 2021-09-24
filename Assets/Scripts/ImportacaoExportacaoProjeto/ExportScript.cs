using System.IO;
using UnityEditor;
using UnityEngine;

public class ExportScript : MonoBehaviour
{
    [SerializeField]
    GameObject render;

    void OnMouseDown()
    {
        string path = EditorUtility.SaveFilePanel("Selecione o local a ser salvo o projeto", "", "ProjectVisEdu", "json");

        if (path.Length != 0)
        {
            var projectJson = GenerateProject();

            using (var streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(projectJson);
            }
        }
    }

    string GenerateProject()
    {
        var project = new ProjectVisEduClass();

        foreach (Transform child in render.transform)
        {
            if (child.name.Contains("CameraSlot"))
            {
                project.Camera.Propriedades = CreatePropCamera(project);
            }
            else if (child.name.Contains("ObjGraficoSlot"))
            {
                ObjetoGraficoProject objGrafPeca = CreateObjGraf(project, child.name);

                if (objGrafPeca != null)
                {
                    project.ObjetosGraficos.Add(objGrafPeca);
                }
            }
        }

        return JsonUtility.ToJson(project, true);
    }

    ObjetoGraficoProject CreateObjGraf(ProjectVisEduClass project, string obgGrafSlot)
    {
        GameObject objGrafico = GameObject.Find(obgGrafSlot);

        string key = GetPeca(obgGrafSlot);
        if (!string.Empty.Equals(key))
        {
            ObjetoGraficoProject objGrafPeca = new ObjetoGraficoProject();
            objGrafPeca.Propriedades = Global.propriedadePecas[key];

            foreach (Transform child in objGrafico.transform)
            {
                if (child.name.Contains("FormasSlot") || child.name.Contains("TransformacoesSlot") || child.name.Contains("IluminacaoSlot"))
                {
                    key = GetPeca(child.name);
                    if (!string.Empty.Equals(key))
                    {
                        var prop = Global.propriedadePecas[key];

                        if (prop != null)
                        {
                            if (prop.Nome.Contains("Cubo"))
                            {
                                objGrafPeca.Forma = CreateFormCubo(prop.Nome, (CuboPropriedadePeca)prop);
                            }
                            else if (prop.Nome.Contains("Poligono"))
                            {
                                objGrafPeca.Forma = CreateFormPoligono(prop.Nome, (PoligonoPropriedadePeca)prop);
                            }
                            else if (prop.Nome.Contains("Spline"))
                            {
                                objGrafPeca.Forma = CreateFormSpline(prop.Nome, (SplinePropriedadePeca)prop);
                            }
                            else if (prop.Nome.Contains("Iluminacao"))
                            {
                                objGrafPeca.Iluminacao = CreateIluminacao(prop);
                            }
                            else if (prop.Nome.Contains("Rotacionar") || prop.Nome.Contains("Escalar") || prop.Nome.Contains("Transladar"))
                            {
                                TransformacaoProject transfObj = CreateTransfProject(prop.Nome, prop);

                                if (transfObj != null)
                                {
                                    objGrafPeca.Transformacoes.Add(transfObj);
                                }
                            }
                        }
                    }
                }
            }
            return objGrafPeca;
        }

        return null;
    }

    FormaProject CreateFormCubo(string formName, CuboPropriedadePeca propPeca)
    {
        return new CuboProject()
        {
            Propriedades = new CuboPropriedadePecaProject()
            {
                Nome = propPeca.Nome,
                Tam = new TamanhoProject()
                {
                    X = EncryptPropPeca(propPeca, Property.TamX),
                    Y = EncryptPropPeca(propPeca, Property.TamY),
                    Z = EncryptPropPeca(propPeca, Property.TamZ)
                },
                Pos = new PosicaoProject()
                {
                    X = EncryptPropPeca(propPeca, Property.PosX),
                    Y = EncryptPropPeca(propPeca, Property.PosY),
                    Z = EncryptPropPeca(propPeca, Property.PosZ)
                },
                NomeCuboAmbiente = propPeca.NomeCuboAmbiente,
                NomeCuboVis = propPeca.NomeCuboVis,
                Cor = propPeca.Cor,
                Textura = propPeca.Textura,
                TipoLuz = propPeca.TipoLuz,
                Intensidade = propPeca.Intensidade.ToString(),
                ValorIluminacao = new ValorIluminacaoProject()
                {
                    X = EncryptPropPeca(propPeca, Property.PosX),
                    Y = EncryptPropPeca(propPeca, Property.PosY),
                    Z = EncryptPropPeca(propPeca, Property.PosZ)
                },
                Distancia = EncryptPropPeca(propPeca, Property.Distancy),
                Angulo = EncryptPropPeca(propPeca, Property.Angle),
                Expoente = EncryptPropPeca(propPeca, Property.Expoent),
                UltimoIndexLuz = propPeca.UltimoIndexLuz,
                Ativo = propPeca.Ativo
            }
        };
    }

    FormaProject CreateFormPoligono(string formName, PoligonoPropriedadePeca propPeca)
    {
        return new PoligonoProject()
        {
            Propriedades = new PoligonoPropriedadePecaProject()
            {
                Nome = propPeca.Nome,
                Tam = new TamanhoProject()
                {
                    X = EncryptPropPeca(propPeca, Property.TamX),
                    Y = EncryptPropPeca(propPeca, Property.TamY),
                    Z = EncryptPropPeca(propPeca, Property.TamZ)
                },
                Pos = new PosicaoProject()
                {
                    X = EncryptPropPeca(propPeca, Property.PosX),
                    Y = EncryptPropPeca(propPeca, Property.PosY),
                    Z = EncryptPropPeca(propPeca, Property.PosZ)
                },
                PoligonoAmbiente = propPeca.PoligonoAmbiente,
                Cor = propPeca.Cor,
                Textura = propPeca.Textura,
                TipoLuz = propPeca.TipoLuz,
                Intensidade = propPeca.Intensidade.ToString(),
                ValorIluminacao = new ValorIluminacaoProject()
                {
                    X = EncryptPropPeca(propPeca, Property.PosX),
                    Y = EncryptPropPeca(propPeca, Property.PosY),
                    Z = EncryptPropPeca(propPeca, Property.PosZ)
                },
                Distancia = EncryptPropPeca(propPeca, Property.Distancy),
                Angulo = EncryptPropPeca(propPeca, Property.Angle),
                Expoente = EncryptPropPeca(propPeca, Property.Expoent),
                UltimoIndexLuz = propPeca.UltimoIndexLuz,
                Ativo = propPeca.Ativo
            }
        };
    }
    FormaProject CreateFormSpline(string formName, PropriedadePeca propPeca)
    {
        return new SplineProject()
        {
            Propriedades = new SplinePropriedadePecaProject()
            {
                Nome = propPeca.Nome,
                Tam = new TamanhoProject()
                {
                    X = EncryptPropPeca(propPeca, Property.TamX),
                    Y = EncryptPropPeca(propPeca, Property.TamY),
                    Z = EncryptPropPeca(propPeca, Property.TamZ)
                },
                Pos = new PosicaoProject()
                {
                    X = EncryptPropPeca(propPeca, Property.PosX),
                    Y = EncryptPropPeca(propPeca, Property.PosY),
                    Z = EncryptPropPeca(propPeca, Property.PosZ)
                },
                Cor = propPeca.Cor,
                Textura = propPeca.Textura,
                TipoLuz = propPeca.TipoLuz,
                Intensidade = propPeca.Intensidade.ToString(),
                ValorIluminacao = new ValorIluminacaoProject()
                {
                    X = EncryptPropPeca(propPeca, Property.PosX),
                    Y = EncryptPropPeca(propPeca, Property.PosY),
                    Z = EncryptPropPeca(propPeca, Property.PosZ)
                },
                Distancia = EncryptPropPeca(propPeca, Property.Distancy),
                Angulo = EncryptPropPeca(propPeca, Property.Angle),
                Expoente = EncryptPropPeca(propPeca, Property.Expoent),
                UltimoIndexLuz = propPeca.UltimoIndexLuz,
                Ativo = propPeca.Ativo
            }
        };
    }

    IluminacaoProject CreateIluminacao(PropriedadePeca propPeca)
    {
        IluminacaoProject IluminacaoProj = new IluminacaoProject();
        IluminacaoProj.Propriedades = CreatePropPeca(propPeca);

        return IluminacaoProj;
    }

    PropriedadeCameraProject CreatePropCamera(ProjectVisEduClass project)
    {
        return new PropriedadeCameraProject()
        {
            Nome = Global.propCameraGlobal.Nome,
            PosX = EncryptPropCam(Property.PosX),
            PosY = EncryptPropCam(Property.PosY),
            PosZ = EncryptPropCam(Property.PosZ),
            FOV = EncryptPropCam(Property.FOV),
            CameraAtiva = Global.propCameraGlobal.CameraAtiva,
            JaIniciouValores = Global.propCameraGlobal.JaIniciouValores,
            ExisteCamera = Global.propCameraGlobal.ExisteCamera,
        };
    }

    PropriedadePecaProject CreatePropPeca(PropriedadePeca propPeca)
    {
        return new PropriedadePecaProject()
        {
            Nome = propPeca.Nome,
            Tam = new TamanhoProject()
            {
                X = EncryptPropPeca(propPeca, Property.TamX),
                Y = EncryptPropPeca(propPeca, Property.TamY),
                Z = EncryptPropPeca(propPeca, Property.TamZ)
            },
            Pos = new PosicaoProject()
            {
                X = EncryptPropPeca(propPeca, Property.PosX),
                Y = EncryptPropPeca(propPeca, Property.PosY),
                Z = EncryptPropPeca(propPeca, Property.PosZ)
            },
            Ativo = propPeca.Ativo,
            Cor = propPeca.Cor,
            Textura = propPeca.Textura,
            TipoLuz = propPeca.TipoLuz,
            Intensidade = propPeca.Intensidade.ToString(),
            ValorIluminacao = new ValorIluminacaoProject()
            {
                X = EncryptPropPeca(propPeca, Property.PosX),
                Y = EncryptPropPeca(propPeca, Property.PosY),
                Z = EncryptPropPeca(propPeca, Property.PosZ)
            },
            Distancia = EncryptPropPeca(propPeca, Property.Distancy),
            Angulo = EncryptPropPeca(propPeca, Property.Angle),
            Expoente = EncryptPropPeca(propPeca, Property.Expoent),
            UltimoIndexLuz = propPeca.UltimoIndexLuz
        };
    }

    TransformacaoProject CreateTransfProject(string transfName, PropriedadePeca propPeca)
    {
        TransformacaoProject transf = null;

        if (transfName.Contains("Escalar")) transf = new EscalarProject();
        else if (transfName.Contains("Rotacionar")) transf = new RotacionarProject();
        else if (transfName.Contains("Transladar")) transf = new TransladarProject();

        if (transf != null)
        {
            transf.Propriedades = CreatePropPeca(propPeca);
            return transf;
        }

        return null;
    }

    string EncryptPropCam(Property prop)
    {
        PropriedadeCamera propCam = Global.propCameraGlobal;

        switch (prop)
        {
            case Property.PosX: return (propCam.ListPropCamLocks.ContainsKey("PosX")) ? Util_VisEdu.Base64Encode(propCam.ListPropCamLocks["PosX"]) : propCam.PosX.ToString();
            case Property.PosY: return (propCam.ListPropCamLocks.ContainsKey("PosY")) ? Util_VisEdu.Base64Encode(propCam.ListPropCamLocks["PosY"]) : propCam.PosY.ToString();
            case Property.PosZ: return (propCam.ListPropCamLocks.ContainsKey("PosZ")) ? Util_VisEdu.Base64Encode(propCam.ListPropCamLocks["PosZ"]) : propCam.PosZ.ToString();
            case Property.FOV: return (propCam.ListPropCamLocks.ContainsKey("FOV")) ? Util_VisEdu.Base64Encode(propCam.ListPropCamLocks["FOV"]) : propCam.FOV.ToString();
            default: return string.Empty;
        }
    }

    string EncryptPropPeca(PropriedadePeca peca, Property propType)
    {
        switch (propType)
        {
            case Property.PosX: return (peca.ListPropLocks.ContainsKey("PosX")) ? Util_VisEdu.Base64Encode(peca.ListPropLocks["PosX"]) : peca.Pos.X.ToString();
            case Property.PosY: return (peca.ListPropLocks.ContainsKey("PosY")) ? Util_VisEdu.Base64Encode(peca.ListPropLocks["PosY"]) : peca.Pos.Y.ToString();
            case Property.PosZ: return (peca.ListPropLocks.ContainsKey("PosZ")) ? Util_VisEdu.Base64Encode(peca.ListPropLocks["PosZ"]) : peca.Pos.Z.ToString();
            case Property.TamX: return (peca.ListPropLocks.ContainsKey("TamX")) ? Util_VisEdu.Base64Encode(peca.ListPropLocks["TamX"]) : peca.Tam.X.ToString();
            case Property.TamY: return (peca.ListPropLocks.ContainsKey("TamY")) ? Util_VisEdu.Base64Encode(peca.ListPropLocks["TamY"]) : peca.Tam.Y.ToString();
            case Property.TamZ: return (peca.ListPropLocks.ContainsKey("TamZ")) ? Util_VisEdu.Base64Encode(peca.ListPropLocks["TamZ"]) : peca.Tam.Z.ToString();
            case Property.Distancy: return (peca.ListPropLocks.ContainsKey("Distancy")) ? Util_VisEdu.Base64Encode(peca.ListPropLocks["Distancy"]) : peca.Distancia.ToString();
            case Property.Angle: return (peca.ListPropLocks.ContainsKey("Angle")) ? Util_VisEdu.Base64Encode(peca.ListPropLocks["Angle"]) : peca.Angulo.ToString();
            case Property.Expoent: return (peca.ListPropLocks.ContainsKey("Expoent")) ? Util_VisEdu.Base64Encode(peca.ListPropLocks["Expoent"]) : peca.Expoente.ToString();
            default: return string.Empty;
        }
    }

    string GetPeca(string slot)
    {
        foreach (var peca in Global.listaEncaixes)
        {
            if (peca.Value.Equals(slot)) return peca.Key;
        }

        return string.Empty;
    }
}