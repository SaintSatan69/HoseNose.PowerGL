using HoseRenderer.ExtraUtils;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoseRenderer
{
    namespace PowerGl
    {
        /// <summary>
        /// For use with the GUI console, when Enter is pressed send the command to this class to act as a wrapper for the ModifyShapeProperty of the main engine safer.
        /// </summary>
        public class ConsoleCommandHandler
        {//This class is dirty and might be overkill/a mess but its easy for me to read/make without dirting the program.cs file, THIS IS AN ABOMINATION I NEED MY VISUAL STUDIO TAKEN FROM ME.
            /// <summary>
            /// an enum for all possible commands doesn't include params but thats for the HELP command
            /// </summary>
            enum CommandTypes
            {
                HELP,
                MOVE,
                ROTATE,
                SCALE,
                STRETCH,
                NEW,
                CONFIG,
            }
            /// <summary>
            /// The Function will parse the given command string and execute the given command if its valid
            /// </summary>
            /// <param name="commandstring"></param>
            public static string ExecutePGLCommand(string commandstring)
            {
                var CMD_SPLAT = commandstring.Split(' ');
                if (CMD_SPLAT.Length > 1) {
                    return CMD_SPLAT[0].ToUpper() switch
                    {
                        "HELP" => ExecuteCommand(CommandTypes.HELP, CMD_SPLAT),
                        "MOVE" => ExecuteCommand(CommandTypes.MOVE, CMD_SPLAT),
                        "ROTATE" => ExecuteCommand(CommandTypes.ROTATE, CMD_SPLAT),
                        "SCALE" => ExecuteCommand(CommandTypes.SCALE, CMD_SPLAT),
                        "STRETCH" => ExecuteCommand(CommandTypes.STRETCH, CMD_SPLAT),
                        "NEW" => ExecuteCommand(CommandTypes.NEW, CMD_SPLAT),
                        "CONFIG" => ExecuteCommand(CommandTypes.CONFIG,CMD_SPLAT),
                        _ => $"{commandstring} is not a valid command",
                    };
                }
                if (CMD_SPLAT.Length == 1 && CMD_SPLAT[0].ToUpper() == "HELP")
                {
                    return ExecuteCommand(CommandTypes.HELP, CMD_SPLAT);
                    
                }
                else
                {
                    return $"No Command Entered";
                }
            }
            /// <summary>
            /// the underlying function that takes the prepared diretive from the public API and runs this nightmare of a function because why not
            /// </summary>
            /// <param name="action"></param>
            /// <param name="CompleteArgs"></param>
            /// <returns></returns>
            private static string ExecuteCommand(CommandTypes action, string[] CompleteArgs)
            {
                //While I could define the X,Y,Z,ShapeNumber from this scope to not have to redefine them, I want to make sure the shape calls don't get murky and collide
                string Execution_status = "";
                if (action == CommandTypes.HELP)
                {
                    if (CompleteArgs.Length > 1)
                    {
                        Execution_status = CompleteArgs[1].ToUpper() switch
                        {
                            "MOVE" => "[HELP] MOVE Command takes [Int] ShapeNumber, [float] X, [float] Y, [float] Z. X,Y,Z are applied inrelation to world state, not where the shape currently is.",
                            "ROTATE" => "[HELP] ROTATE Command takes [int] ShapeNumber, [float] X, [float] Y, [float] Z. X,Y,Z are the degress of rotation applied.",
                            "SCALE" => "[HELP] SCALE Command takes [int] ShapeNumber, [float] Size. Size is applied to X,Y,Z.",
                            "STRECH" => "[HELP] STRETCH command takes [int] ShapeNumber, [float] X, [float] Y, [float] Z. Value of '*' will cause the engine to use the current strech in the event you only" +
                                                                " want to modify a stretch on a certain axis, all 3 axis are required.",
                            "NEW" => "[HELP][WARNING EXPERIEMENTAL] NEW Command Takes [float] X, [float] Y, [float] Z, [string NO SPACES] PathToVertexShader, [string NO SPACES] PathToFragmentShader. The engine will assign it a shapenumber automatically.",
                            "CONFIG" => "[HELP] Config Command Accepts [int] WindowX, [int] WindowY, [string] WindowTitle, [Bool] VsyncEnabled",
                            _ => "[HELP] Invalid HELP Command",
                        };
                    }
                    else
                    {
                        var enum_keys = Enum.GetValues(typeof(CommandTypes));
                        Execution_status = $"[HELP] All Commands are:{Environment.NewLine}";
                        for (int i = 0; i < enum_keys.Length; i++)
                        {
                            Execution_status += $"{enum_keys.GetValue(i)}{Environment.NewLine}";
                        }
                    }
                        
                }
                if (action == CommandTypes.MOVE)
                {
                    int shape_num = 0;
                    float X = 0.0f;
                    float Y = 0.0f;
                    float Z = 0.0f;
                    try
                    {
                        //To find all the params given to the shape and make sure there is no missing ones
                        for (int i = 1; i < CompleteArgs.Length; i++)
                        {

                            if (CompleteArgs[i].ToUpper() == "SHAPENUMBER")
                            {
                                shape_num = Convert.ToInt32(CompleteArgs[i + 1]);
                            }
                            if (CompleteArgs[i].ToUpper() == "X")
                            {
                                X = Convert.ToSingle(CompleteArgs[i + 1]);
                            }
                            if(CompleteArgs[i].ToUpper() == "Y")
                            {
                                Y = Convert.ToSingle(CompleteArgs[i + 1]);
                            }
                            if (CompleteArgs[i].ToUpper() == "Z")
                            {
                                Z = Convert.ToSingle(CompleteArgs[i + 1]);
                            }
                        }
                        int _engine_return = MainRenderer.ModifyShapeProperty(shape_num, "Position", X, Y, Z, null, null, null,null);
                        if (_engine_return == 0)
                        {
                            Execution_status = $"MOVE Command Completed Sucessfully on {shape_num}, New pos {X}-{Y}-{Z}";
                        }
                        else
                        {
                            Execution_status = $"Engine Call FAILED returned code {_engine_return}";
                        }
                    }
                    catch(Exception ex)
                    {
                        if (ex.Message.Contains("format")) 
                        {
                            Execution_status = "ERROR Failed to convert a parameter into the correct format make sure you have entered it correctly";
                        }
                        else
                        {
                            Execution_status = $"ERROR {ex.Message}";
                        }
                    }
                }
                if (action == CommandTypes.ROTATE)
                {
                    int shape_num = 0;
                    float X = 0.0f;
                    float Y = 0.0f;
                    float Z = 0.0f;
                    try
                    {
                        for (int i = 0; i < CompleteArgs.Length; i++) {
                            if (CompleteArgs[i].ToUpper() == "SHAPENUMBER")
                            {
                                shape_num = Convert.ToInt32(CompleteArgs[i + 1]);
                            }
                            if (CompleteArgs[i].ToUpper() == "X")
                            {
                                X = Convert.ToSingle(CompleteArgs[i + 1]);
                            }
                            if (CompleteArgs[i].ToUpper() == "Y")
                            {
                                Y = Convert.ToSingle(CompleteArgs[i + 1]);
                            }
                            if (CompleteArgs[i].ToUpper() == "Z")
                            {
                                Z = Convert.ToSingle(CompleteArgs[i + 1]);
                            }
                        }
                        int _engine_return = MainRenderer.ModifyShapeProperty(shape_num,"Rotate",X,Y,Z,null,null,null,null);
                        if (_engine_return == 0)
                        {
                            Execution_status = $"ROTATE Command completed successfully on {shape_num}, new rot {X}-{Y}-{Z}";
                        }
                        else
                        {
                            Execution_status = $"Engine call FAILED returned code {_engine_return}";
                        }
                    }
                    catch(Exception ex)
                    {
                        if (ex.Message.Contains("format"))
                        {
                            Execution_status = "ERROR Failed to convert a parameter into the correct format make sure you have entered it correctly";
                        }
                        else
                        {
                            Execution_status = $"Error {ex.Message}";
                        }
                    }
                }
                if (action == CommandTypes.SCALE)
                {
                    int shape_num = 0;
                    float size = 0.0f;
                    try
                    {
                        for (int i = 0; i < CompleteArgs.Length; i++) {
                            if (CompleteArgs[i].ToUpper() == "SHAPENUMBER")
                            {
                                shape_num = Convert.ToInt32(CompleteArgs[i + 1]);
                            }
                            if (CompleteArgs[i].ToUpper() == "SIZE")
                            {
                                size = Convert.ToSingle(CompleteArgs[i + 1]);
                            }
                        }
                        int _engine_return = MainRenderer.ModifyShapeProperty(shape_num,"Scale",0,0,0,null,null,null,size);
                        if (_engine_return == 0)
                        {
                            Execution_status = $"SCALE Command Completed Successfully {shape_num}, new size is {size}";
                        }
                        else
                        {
                            Execution_status = $"Engine Call FAILED returned code {_engine_return}";
                        }
                    }
                    catch (Exception ex) 
                    {
                        if (ex.Message.Contains("format"))
                        {
                            Execution_status = "ERROR Failed to convert a paremeter into the correct format make sure you have entered it correctly";
                        }
                        else
                        {
                            Execution_status = $"Error {ex.Message}";
                        }
                    }

                }
                if (action == CommandTypes.STRETCH)
                {
                    int shape_num = 0;
                    float X = 0.0f;
                    float Y = 0.0f;
                    float Z = 0.0f;
                    string _hidden_stretch = "";
                    try
                    {
                        for (int i = 0; i < CompleteArgs.Length; i++)
                        {
                            if (CompleteArgs[i].ToUpper() == "SHAPENUMBER")
                            {
                                shape_num = Convert.ToInt32(CompleteArgs[i + 1]);
                            }
                            if (CompleteArgs[i].ToUpper() == "X")
                            {
                                if (!(CompleteArgs[i + 1] == "*")) {
                                    X = Convert.ToSingle(CompleteArgs[i + 1]);
                                    _hidden_stretch += $"{X}-";
                                }
                                else
                                {
                                    X = 0.69420554f;
                                    _hidden_stretch += "*-";
                                }
                            }
                            if (CompleteArgs[i].ToUpper() == "Y")
                            {
                                if (!(CompleteArgs[i + 1] == "*")) {
                                    Y = Convert.ToSingle(CompleteArgs[i + 1]);
                                    _hidden_stretch += $"{Y}-";
                                }
                                else
                                {
                                    Y = 0.69420554f;
                                    _hidden_stretch += "*-";
                                }
                            }
                            if (CompleteArgs[i].ToUpper() == "Z")
                            {
                                if (!(CompleteArgs[i + 1] == "*")) {
                                    Z = Convert.ToSingle(CompleteArgs[i + 1]);
                                    _hidden_stretch += $"{Z}";
                                }
                                else
                                {
                                    Z = 0.69420554f;
                                    _hidden_stretch += "*";
                                }
                            }
                        }
                        int _engine_return = MainRenderer.ModifyShapeProperty(shape_num, "Stretch", X, Y, Z, null, null, null, null);
                        if (_engine_return == 0)
                        {
                            Execution_status = $"STRETCH Command completed successfully on {shape_num}, new stretch {_hidden_stretch}";
                        }
                        else
                        {
                            Execution_status = $"Engine call FAILED returned code {_engine_return}";
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("format"))
                        {
                            Execution_status = "ERROR Failed to convert a parameter into the correct format make sure you have entered it correctly";
                        }
                        else
                        {
                            Execution_status = $"Error {ex.Message}";
                        }
                    }
                }
                if (action == CommandTypes.NEW)
                {
                    float X = 0.0f;
                    float Y = 0.0f;
                    float Z = 0.0f;
                    string shader_path = "";
                    string frag_path = "";
                    string text_path = "";
                    try
                    {
                        for (int i = 0; i < CompleteArgs.Length; i++) 
                        {
                            if (CompleteArgs[i].ToUpper() == "X")
                            {
                                X = Convert.ToSingle(CompleteArgs[i + 1]);
                            }
                            if (CompleteArgs[i].ToUpper() == "Y")
                            {
                                Y = Convert.ToSingle(CompleteArgs[i + 1]);
                            }
                            if (CompleteArgs[i].ToUpper() == "Z")
                            {
                                Z = Convert.ToSingle(CompleteArgs[i + 1]);
                            }
                            if (CompleteArgs[i].ToUpper() == "SHADER")
                            {
                                shader_path = CompleteArgs[i + 1];
                            }
                            if (CompleteArgs[i].ToUpper() == "FRAGMENT")
                            {
                                frag_path = CompleteArgs[i + 1];
                            }
                            if (CompleteArgs[i].ToUpper() == "TEXTURE")
                            {
                                text_path = CompleteArgs[i + 1];
                            }
                        }
                        int _engine_return = MainRenderer.ModifyShapeProperty((MainRenderer.GlobalShapeCount),"NEW",X,Y,Z,shader_path,frag_path,text_path,null);
                        if (_engine_return == 0)
                        {
                            Execution_status = "NEW command completed successfully";
                        }
                        else
                        {
                            Execution_status = $"Engine call FAILED with exit code {_engine_return}";
                        }
                    }
                    catch (Exception ex) 
                    {
                        if (ex.Message.Contains("format"))
                        {
                            Execution_status = "ERROR Failed to convert a parameter into the correct format make sure you have entered it correctly";
                        }
                        else
                        {
                            Execution_status = $"Error {ex.Message}";
                        }
                    }
                }
                if (action == CommandTypes.CONFIG)
                {
                    bool IsModify = false;
                    for(int i = 0; i < CompleteArgs.Length; i++)
                    {
                        if (CompleteArgs[i].ToUpper() == "SAVE")
                        {
                            try
                            {
                                MainRenderer.Config.SaveConfig();
                                Execution_status = "Config Saved Successfully";
                            }
                            catch(Exception ex)
                            {
                                Execution_status = $"Config Save Failed Exception: {ex.Message}";
                            }
                        }
                        if (CompleteArgs[i].ToUpper() == "REVERT")
                        {
                            MainRenderer.RevertConfig();
                        }
                        if (CompleteArgs[i].ToUpper() == "APPLY")
                        {
                            try
                            {
                                MainRenderer.ApplyConfig();
                                Execution_status = "Config Applied Sucessfully";
                            }
                            catch( Exception ex)
                            {
                                Execution_status = $"Config Apply Failed Exception: {ex.Message}";
                            }
                        }
                        if (CompleteArgs[i].ToUpper() == "MODIFY")
                        {
                            IsModify = true;
                            break;
                        }
                    }
                    if (IsModify)
                    {
                        int num_parts = CompleteArgs.Length - 2;
                        string[] Properties = new string[num_parts / 2];
                        string[] Values = new string[num_parts / 2];
                        int num_operations = num_parts / 2;
                        int propertyindexer = 0;
                        int valueindexer = 0;
                        for(int j = 0; j < CompleteArgs.Length; j++)
                        {
                            if (CompleteArgs[j].ToUpper() == "WINDOWX")
                            {
                                Properties[propertyindexer] = CompleteArgs[j];
                                Values[valueindexer] = CompleteArgs[j + 1];
                                propertyindexer++;
                                valueindexer++;
                            }
                            if (CompleteArgs[j].ToUpper() == "WINDOWY")
                            {
                                Properties[propertyindexer] = CompleteArgs[j];
                                Values[valueindexer] = CompleteArgs[j + 1];
                                propertyindexer++;
                                valueindexer++;
                            }
                        }
                        try
                        {
                            MainRenderer.UpdateRunningConfig(num_parts, Properties, Values);
                            Execution_status = "Modifying The Running Config Completed Successfully, Please Apply Or Save Config";
                        }
                        catch (Exception ex) 
                        {
                            Execution_status = $"FAILED to update running config for reasion {ex.Message}";
                        }
                    }
                }
                return Execution_status;
            }
        }
    }
}
