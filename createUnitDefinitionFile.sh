TEMP_FOLDER=tmp
REPO_URL=https://github.com/beyond-all-reason/Beyond-All-Reason.git
REP_SUBFOLDER=units
LOCAL_FOLDER=git
CONVERTER_EXE=../src/LuaToJsonConverter/bin/Debug/net10.0/LuaToJsonConverter.exe
CONVERT_FOLDER=converted
RESULT_ZIP=units.zip

UNIT_FOLDER=$LOCAL_FOLDER/$REP_SUBFOLDER

# create tmp environment
echo Create temp folder...
mkdir $TEMP_FOLDER
cd $TEMP_FOLDER
echo

# checkout only unit files
echo Checkout...
git clone --no-checkout --filter=blob:none --depth=1 --sparse $REPO_URL $LOCAL_FOLDER
cd $LOCAL_FOLDER
git sparse-checkout init --cone
git sparse-checkout set $REP_SUBFOLDER
git checkout
cd ..
echo

# remove not needed files
echo Remove non building lua files...
cd $LOCAL_FOLDER
## remove non lua files
### dry-run: find tmp_bar/units -type f ! -name "*.lua" -ls
find $REP_SUBFOLDER -type f ! -name "*.lua" -delete

## remove non building unit files -> lua files doesn't contain property "yardmap"
### dry-run: grep -rL -- 'yardmap =' $REP_SUBFOLDER | sort
grep -rLZ -- 'yardmap =' $REP_SUBFOLDER | xargs -0 rm -v

## remove empty folders
find $REP_SUBFOLDER/* -type d -empty -delete
cd ..
echo

# convert lua files to json files
echo Converting lua to json...
$CONVERTER_EXE $LOCAL_FOLDER/$REP_SUBFOLDER $CONVERT_FOLDER
echo

# zip unit folder
echo Zipping...
cd $CONVERT_FOLDER
zip -r -9 ../../$RESULT_ZIP .
cd ..
echo

# clean up
echo Cleanup...
cd ..
rm -rf $TEMP_FOLDER

echo

echo Done.