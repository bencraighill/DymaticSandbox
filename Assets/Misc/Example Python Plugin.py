"""
BEGIN_PLUGIN_METADATA
Name: "My Awesome Plugin"
Description: "This plugin does amazing things!"
Icon: "./test_icon.png"
Author: "John Doe"
Company Name: "My Company"
Version: "1.0.0"
Dependencies: "random, string"
Build Date: "20/04/2024"
Engine Version Requirements: "25.1.0"
Legal Copyright: "© 2024 My Company"
Legal Trademarks: "My Software®, All Rights Reserved"
License: |
        Unless required by applicable law or agreed to in writing, software
        distributed under the License is distributed on an "AS IS" BASIS,
        WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
        See the License for the specific language governing permissions and
        limitations under the License.
END_PLUGIN_METADATA
"""

import Dymatic
import webbrowser
import random
import string
import time

global pluginIcon

def on_clicked():
    print("We clicked a button!")

def on_action():
    print("File action callback!")

def popup_render():
    Dymatic.UI.Text("Testing!")
    Dymatic.UI.Image(pluginIcon, Dymatic.Vector2(50,50))

def taskbarButtonCallback():
    print("Hiiiiiiiya!")

def DymaticOnLoad(context):
    global pluginIcon
    print(context)
    pluginIcon = Dymatic.Plugin.LoadTexture(context, "./test_icon.png")
    print(pluginIcon)
    #Dymatic.Editor.Popup("Python Message", "Hello world from Python!", False, pluginIcon)
    color = Dymatic.Vector3(1.0, 0.0, 1.0)

    Dymatic.Taskbar.AddThumbnailButton(context, pluginIcon, "Python Button", taskbarButtonCallback, True)

    #Dymatic.Editor.Notification("Python Notification", "Hello world from Python!", 5.0, False, pluginIcon, color)
    Dymatic.Editor.Popup("Python Notification", "Hello world from Python!", [Dymatic.Button("Test Button", on_clicked)], False, pluginIcon, popup_render, Dymatic.Vector2(200, 200))
    Dymatic.Editor.Notification("Python Notification", "Hello world from Python!", [Dymatic.Button("Test Button", on_clicked)], 0.0, False)

    Dymatic.ContentBrowser.RegisterFileAction(context, ".dymesh", "Optimize Mesh", on_action, "Test Python Plugin")

    Dymatic.Editor.Transaction("Python Transaction")

def DymaticOnUpdate(test):
    if (Dymatic.Input.IsKeyPressed(Dymatic.Key.LeftControl) and Dymatic.Input.IsKeyPressed(Dymatic.Key.L)):
        print("Ctrl-L was pressed!")

    #print(Dymatic.Scene.FindEntityByName("Static Mesh").GetComponent(Dymatic.StaticMeshComponent).Mesh.Type)
    #print(f"On Update:  seconds...")

    spriteEntity = Dymatic.Scene.FindEntityByName("Python Entity")

loading_progress = 0 
mesh_asset = Dymatic.Asset()
mesh_scale = Dymatic.Vector3(1.0)

def DymaticOnUIRender(renderStage):
    if renderStage == Dymatic.UIRenderStage.Main:
        global pluginIcon
        global loading_progress
        global mesh_asset
        global mesh_scale

        Dymatic.UI.BeginWindow("My Python Window")
        Dymatic.UI.Image(pluginIcon, Dymatic.Vector2(100, 100), Dymatic.Vector4(1,0,0,1), Dymatic.Vector4(0,0,1,0.5))

        [modified, mesh] = Dymatic.UI.AssetSelectionDropdown(Dymatic.AssetType.Mesh, mesh_asset)
        if (modified):
            mesh_asset = mesh

        mesh_scale = Dymatic.UI.InputVector("Scale", mesh_scale, Dymatic.Vector3(1.0))

        if (Dymatic.UI.Button("Add 100 meshes")):
            for i in range(1, 100):                
                entity = Dymatic.Scene.CreateEntity("My Python Entity")
                entity.AddComponent(Dymatic.StaticMeshComponent).Mesh = mesh_asset
                entity.Tag = ''.join(random.choice(string.ascii_uppercase + string.digits) for _ in range(10))
                entity.Translation = Dymatic.Vector3(random.randint(-10, 10) * 100.0, 0.0, random.randint(-10, 10) * 100.0)
                entity.Scale = mesh_scale
                #print(entity.Translation)
        
        if (Dymatic.UI.Button("Flash Window Icon")):
            time.sleep(1)
            Dymatic.Taskbar.FlashIcon()

        if (Dymatic.UI.Button("Start Loading Status")):
            Dymatic.Taskbar.SetLoadingStatus(True)

        if (Dymatic.UI.Button("Stop Loading Status")):
            Dymatic.Taskbar.SetLoadingStatus(False)

        modified, loading_progress = Dymatic.UI.DragInt("Loading Progress", loading_progress, 1.0, 1, 100)
        if modified:
            Dymatic.Taskbar.SetLoadingProgress(loading_progress / 100.0)            

        Dymatic.UI.EndWindow()
    elif renderStage == Dymatic.UIRenderStage.PluginPreferences:
        Dymatic.UI.Text("Hello... This is my test icon:")
        Dymatic.UI.Image(pluginIcon, Dymatic.Vector2(100, 100), Dymatic.Vector4(1,0,0,1), Dymatic.Vector4(0,1,0,0.5))
    
def DymaticOnUnload():
    print("Unloaded!")