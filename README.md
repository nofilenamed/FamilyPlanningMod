# FamilyPlanningMod
This is a Stardew Valley mod called Family Planning. Family Planning allows you to customize the maximum number of children you can have and their genders. Currently, you can have zero to four children.

## How to use:

After you've loaded the singleplayer save file you want the mod to change, go to the SMAPI console and use the command "set_number_of_children <value>". This will change the maximum number of children you can have to the value you entered, from zero to four. There's also a command "get_number_of_children" to check what the value is currently set to.

For example, if you wanted to have the max number of children possible, four, you would load your save file and type "set_number_of_children 4" into the console. Your spouse will then be able to prompt you about having children until you reach four.

Also, you will now be able to choose your child's gender when you name them. You could have one son, or three daughters, or two sons and two daughters, or no children at all, etc, the choice is yours.

## Child Appearance Mods:

If you're using other mods to change the appearance of your children, this mod will be affected. This mod doesn't change the way child sprites are generated, so any mod which directly replaces the xnb file will lead to children being identical. For Content Patcher mods, you can personally edit the content.json file to patch different appearances for each child.

## Compatibility:

Works with Stardew Valley 1.3 beta on Linux/Mac/Windows.
Only works in single player for the current version.
This mod uses Harmony, so there may be interference with other mods using Harmony to patch the same methods. If you notice any issues, please let me know.

## More details:

The reason why the mod currently limits your to four children maximum is because it doesn't edit the number of beds in your house. Therefore, all of your children need to share the two existing beds. Two children can fit in a bed together, so four children is the limit (unless I update this mod to add more beds). Also, children will attempt to share a bed with a sibling of the same gender when possible.

For the 1.0.0 version, your spouse won't get dialogue acknowledging the additional children (besides the generic adoption dialogue for gay couples). This is something I'm planning to update.
