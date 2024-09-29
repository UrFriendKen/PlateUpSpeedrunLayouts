using KitchenData;
using KitchenMods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace KitchenSpeedrunLayouts
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct CRefreshLayoutsActivator : IApplianceProperty, IComponentData, IModComponent { }
}
