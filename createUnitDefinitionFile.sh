TEMP_FOLDER=tmp
REPO_URL=https://github.com/beyond-all-reason/Beyond-All-Reason.git
REP_UNITS_SUBFOLDER=units
REP_UNITPICS_SUBFOLDER=unitpics
GIT_FOLDER=git
CONVERTER_EXE=../src/BarFileConverter/bin/Debug/net10.0/BarFileConverter.exe
CONVERT_FOLDER=converted
RESULT_ZIP=units.zip

UNIT_FOLDER=$GIT_FOLDER/$REP_UNITS_SUBFOLDER
UNITPICS_FOLDER=$GIT_FOLDER/$REP_UNITPICS_SUBFOLDER

UNIT_FOLDER_CONVERTED=$CONVERT_FOLDER/$REP_UNITS_SUBFOLDER
UNITPICS_FOLDER_CONVERTED=$CONVERT_FOLDER/$REP_UNITPICS_SUBFOLDER


# create tmp environment
echo Create temp folder...
mkdir $TEMP_FOLDER
cd $TEMP_FOLDER
mkdir $CONVERT_FOLDER
echo Done.
echo

# units
echo Checkout...
git clone --no-checkout --filter=blob:none --depth=1 --sparse $REPO_URL $GIT_FOLDER
cd $GIT_FOLDER
git sparse-checkout init --cone
git sparse-checkout set $REP_UNITS_SUBFOLDER
git checkout
cd ..
echo Done.
echo

echo Converting lua to json...
$CONVERTER_EXE lua $UNIT_FOLDER $UNIT_FOLDER_CONVERTED
echo Done.
echo

# unitpics
cd $GIT_FOLDER
git sparse-checkout init --cone
git sparse-checkout set $REP_UNITPICS_SUBFOLDER
git checkout
cd ..

echo Converting dds to jpg...
$CONVERTER_EXE dds $UNITPICS_FOLDER $UNITPICS_FOLDER_CONVERTED
echo Done.
echo

# zip unit folder
echo Zipping...
cd $CONVERT_FOLDER
zip -r -9 ../$RESULT_ZIP .
cd ..
echo Done.
echo

# copy to www
cp $RESULT_ZIP ../src/BarBlueprintEditorWeb/BarBlueprintEditorWeb/wwwroot/resources/.

# clean up
echo Cleanup...
cd ..
#rm -rf $TEMP_FOLDER

echo Done.
echo