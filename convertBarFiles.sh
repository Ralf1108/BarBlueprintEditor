TEMP_FOLDER=tmp_t
REPO_URL=https://github.com/beyond-all-reason/Beyond-All-Reason.git
REP_UNITS_SUBFOLDER=units
REP_UNITPICS_SUBFOLDER=unitpics
GIT_FOLDER=git
CONVERTER_EXE=../src/BarFileConverter/bin/Debug/net10.0/BarFileConverter.exe
CONVERTED_FOLDER=converted
RESULT_ZIP=units.zip

UNIT_DEFINITIONS_FILE=webUnitDefinitions.json
UNIT_SCHEMA_FILE=commonSchema.json
UNIT_CS_CLASSES_FILE=csharp.cs

UNIT_FOLDER=$GIT_FOLDER/$REP_UNITS_SUBFOLDER
UNITPICS_FOLDER=$GIT_FOLDER/$REP_UNITPICS_SUBFOLDER

UNIT_FOLDER_CONVERTED=$CONVERTED_FOLDER/$REP_UNITS_SUBFOLDER
UNITPICS_FOLDER_CONVERTED=$CONVERTED_FOLDER/$REP_UNITPICS_SUBFOLDER


## create tmp environment
echo Create temp folder...
mkdir $TEMP_FOLDER
cd $TEMP_FOLDER
mkdir $CONVERTED_FOLDER
echo Done.
echo

echo Fetching unit defintions...
$CONVERTER_EXE web --target $UNIT_DEFINITIONS_FILE
echo Done.
echo


echo Checkout...
git clone --no-checkout --filter=blob:none --depth=1 --sparse $REPO_URL $GIT_FOLDER

## units
cd $GIT_FOLDER
git sparse-checkout init --cone
git sparse-checkout set $REP_UNITS_SUBFOLDER
git checkout
cd ..
echo Done.
echo

echo Converting lua to json...
$CONVERTER_EXE lua --source $UNIT_FOLDER --target $UNIT_FOLDER_CONVERTED --unitDefinitions $UNIT_DEFINITIONS_FILE
echo Done.
echo

# generate C# unit classes
echo Generate C# unit classes...
$CONVERTER_EXE schema --source $UNIT_FOLDER_CONVERTED --target $UNIT_SCHEMA_FILE
$CONVERTER_EXE csharp --source $UNIT_SCHEMA_FILE --target $UNIT_CS_CLASSES_FILE
echo Done.
echo

# unitpics
cd $GIT_FOLDER
git sparse-checkout init --cone
git sparse-checkout set $REP_UNITPICS_SUBFOLDER
git checkout
cd ..

echo Converting dds to jpg...
$CONVERTER_EXE dds --source $UNITPICS_FOLDER --target $UNITPICS_FOLDER_CONVERTED --unitDefinitions $UNIT_DEFINITIONS_FILE
echo Done.
echo

# zip unit folder
echo Zipping...
cd $CONVERTED_FOLDER
zip -r -9 ../$RESULT_ZIP .
cd ..
echo Done.
echo

# copy to solution www assets
cp $RESULT_ZIP ../src/BarBlueprintEditorWeb/BarBlueprintEditorWeb/wwwroot/resources/.

# clean up
echo Cleanup...
cd ..
#rm -rf $TEMP_FOLDER

echo Done.
echo