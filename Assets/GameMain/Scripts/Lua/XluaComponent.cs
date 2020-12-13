using StarForce;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityGameFramework.Runtime;
using XLua;

public class XluaComponent : GameFrameworkComponent
{
//    const string init_script = @"collectgarbage('setpause', 100)
//collectgarbage('setstepmul', 5000)

//function cs_call_new_script_ins(name)
//    return require(name)()
//end";


//    public const string dispose_xlua_manager_script = @"local xluatool = require('xluatool')
//if xluatool then
//    xluatool.onXLuaManagerDestroy()
//end";


//    public const string dispose_script = @"local xluatool = require('xluatool')
//if xluatool then
//    xluatool.clearCustomRefOnDispose()
//end
//";


//    public const string dispose_remain_ref_script = @"local xluatool = require('xluatool')
//if xluatool then
//    xluatool.printFuncRefByCSharp()
//end";

    const string Lua = "Lua";
    public const string LuaTxtExt = ".lua.txt";
    public const string LuaExt = ".lua";

    Func<string, LuaTable> new_script_ins_call = null;

    Dictionary<string, string> m_luaFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    bool m_bInit = false;
    public LuaEnv LuaEnv
    {
        private set;
        get;
    }

    private float m_lastGCTime;
    public const float GCInterval = 1;

    private void Start()
    {
        LuaEnv = new LuaEnv();
        LuaEnv.CustomLoader loader = OriginalLuaLoader;
        LuaEnv.AddLoader(loader);
        InitLuaPath();
        m_bInit = true;
    }

    void OnDestroy()
    {
        new_script_ins_call = null;
        if (LuaEnv != null)
        {
            LuaEnv.Tick();
        }
    }

    void InitLuaPath()
    {
        var luaFiles = Directory.GetFiles(Application.dataPath + "/GameMain/Lua", "*", SearchOption.AllDirectories); ;
        string luaRePathFormat = Lua + "/{0}";
        foreach (var item in luaFiles)
        {
            string fileName;
            if (CastValidLuaFileName(item, out fileName))
            {
                if (!m_luaFiles.ContainsKey(fileName))
                {
                    m_luaFiles[fileName] = string.Format(luaRePathFormat, item.Replace("\\","/"));
                }
                else
                {
                    throw new System.Exception(string.Format("Lua文件名冲突,Lua子目录下的所有lua文件不能同名:\n{0}\n{1}", m_luaFiles[fileName], string.Format(luaRePathFormat, item)));
                }
            }
        }
    }

    bool CastValidLuaFileName(string filePath, out string fileName)
    {
        bool valid;
        if (filePath.EndsWith(LuaTxtExt))
        {
            fileName = Path.GetFileName(filePath);
            fileName = fileName.Substring(0, fileName.Length - LuaTxtExt.Length);
            valid = true;
        }
        else if (filePath.EndsWith(LuaExt))
        {
            fileName = Path.GetFileName(filePath);
            fileName = fileName.Substring(0, fileName.Length - LuaExt.Length);
            valid = true;
        }
        else
        {
            fileName = null;
            valid = false;
        }
        return valid;
    }

    // 指定加载lua脚本
    byte[] OriginalLuaLoader(ref string luaFileName)
    {
        if (!m_bInit)
        {
            Debug.LogError("no init before use");
            return null;
        }

        if (string.IsNullOrEmpty(luaFileName))
        {
            return null;
        }
        string rePath;
        if (m_luaFiles.TryGetValue(luaFileName, out rePath))
        {
            string luaFilePath = AssetUtility.GetLuaTxt(luaFileName);
            File.ReadAllBytes(rePath);
            byte[] bytes = File.ReadAllBytes(rePath);         
            return CastUTF8Bom(bytes);
        }
        return null;
    }

    static byte[] UTF8BomBytes = new byte[3] { 0xEF, 0xBB, 0xBF };
    public static byte[] CastUTF8Bom(byte[] data)
    {
        if (data != null && data.Length >= 3)
        {
            if (data[0] == UTF8BomBytes[0] && data[1] == UTF8BomBytes[1] && data[2] == UTF8BomBytes[2])
            {
                byte[] resData = new byte[data.Length - 3];
                Buffer.BlockCopy(data, 3, resData, 0, resData.Length);
                return resData;
            }
        }
        return data;
    }

    /// <summary>
    /// void Tick()： 
    /// 清除Lua的未手动释放的LuaBase（比如，LuaTable， LuaFunction），
    /// 以及其它一些事情。需要定期调用，比如在MonoBehaviour的Update中调用。
    /// </summary>
    void Update()
    {
        float time = Time.unscaledTime;
        if (time - m_lastGCTime > GCInterval)
        {
            LuaEnv.Tick();
            m_lastGCTime = time;
        }
    }

    public void Dispose()
    {
        if (LuaEnv != null)
        {
            try
            {
                LuaEnv.GC();
                LuaEnv.Dispose();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                if (e.Message.Contains("LuaEnv"))
                {
                    Debug.LogError("c#中还持有lua的回调如下:");
                }
            }
        }
        LuaEnv = null;
        GC.SuppressFinalize(this);
    }
}