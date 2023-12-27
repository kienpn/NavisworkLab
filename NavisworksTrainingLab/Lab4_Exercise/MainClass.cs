using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.DocumentParts;
using Autodesk.Navisworks.Api.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Autodesk.Navisworks.Api.Application;

namespace Lab4_Exercise
{
    [PluginAttribute("Lab4_Exercise", "Hicas", DisplayName = "Lab4", ToolTip = "Clash Detection")]
    public class MainClass : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            // current document
            Document doc = Application.ActiveDocument;
            // get current selected items
            ModelItemCollection selectedItems = doc.CurrentSelection.SelectedItems;
            if (selectedItems.Count == 0)
            {
                MessageBox.Show(Application.Gui.MainWindow, "No items selected.");
                return 0;
            }

            // model items collection-1
            ModelItemCollection itemCollection = new ModelItemCollection();

            // get appended models 
            DocumentModels models = doc.Models;

            // each model
            foreach (Model model in models)
            {
                // collect all items from the mode1
                // add to model item collection-1
                itemCollection.AddRange(ItemsFromRoot(model));
            }

            // model item collection-2
            ModelItemCollection itemsToColor = new ModelItemCollection();


            // each item from the current selected items
            foreach (ModelItem selectedItem in selectedItems)
            {
                // get item2 bounding box
                BoundingBox3D selectedBox = selectedItem.BoundingBox(true);

                // each item from model item collection-1
                foreach (ModelItem otherItem in itemCollection)
                {
                    if (otherItem.InstanceGuid == selectedItem.InstanceGuid)
                    {
                        continue;
                    }

                    // get item1 bounding box
                    BoundingBox3D otherBox = otherItem.BoundingBox(true);          
                    // check intersection of box1 vs box2
                    if (otherBox.Intersects(selectedBox))
                    {
                        // item add to model item collection-2
                        itemsToColor.Add(otherItem);
                    }
                }
            }
            // change the color of model item collection-2 items 
            doc.Models.OverridePermanentColor(itemsToColor, Color.Green);
            return 0;
        }

        // get descendants from model item
        public IEnumerable<ModelItem> ItemsFromRoot(Model model)
        {
            // collect all descendants geometric items from a model 
            return model.RootItem.Descendants.Where(x => x.HasGeometry);
        }
    }
}
