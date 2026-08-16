# District Empire w Unity - instrukcja od zera

Ten plik opisuje, jak ponownie pobrać, uruchomić, przetestować i zbudować
lokalny prototyp District Empire. Obecna wersja nie wymaga backendu, GPS,
reklam ani płatności.

## Wymagania

- Unity Hub.
- Unity 6.3 LTS: **6000.3.5f1**.
- Android Build Support.
- Android SDK & NDK Tools.
- OpenJDK.
- Git.

Folder **unity/** jest kompletnym projektem. Nie twórz wcześniej pustego
projektu Unity.

## Pobranie projektu

Nowa instalacja:

~~~sh
mkdir -p ~/AI
cd ~/AI
git clone https://github.com/disordermail/DistrictEmpire.git
cd DistrictEmpire
~~~

Aktualizacja:

~~~sh
cd ~/AI/DistrictEmpire
git status
git pull origin main
~~~

Nie wykonuj **git pull**, jeżeli masz niezapisane zmiany, których nie chcesz
utracić.

## Otwarcie Unity

W Unity Hub wybierz **Add > Add project from disk** i wskaż:

~~~text
/Users/pwd/AI/DistrictEmpire/unity
~~~

Uruchomienie z terminala:

~~~sh
open -na "/Applications/Unity/Hub/Editor/6000.3.5f1/Unity.app" --args \
  -projectPath /Users/pwd/AI/DistrictEmpire/unity
~~~

Przy pierwszym uruchomieniu poczekaj na zakończenie importu i kompilacji.

## Scena prototypu

Otwórz scenę:

~~~text
Assets/DistrictEmpire/Presentation/Scenes/DistrictEmpireVerticalSlice.unity
~~~

Naciśnij **Play**.

Jeżeli sceny brakuje, wybierz:

~~~text
District Empire > Setup Vertical Slice Scene
~~~

Ta komenda generuje scenę od nowa, więc nie używaj jej bez potrzeby.

## Test wyglądu

W zakładce **Game** wybierz **Free Aspect** i ustaw pionowe, wąskie okno.
Zmieniaj jego wymiary i sprawdź, czy:

- tekst nie nachodzi na inne elementy,
- nazwy przycisków nie zawijają się po jednej literze,
- dolna nawigacja pozostaje widoczna,
- listy można przewijać,
- layout nie przeskakuje po kliknięciu.

Telefon testowy nagrywał ekran w **1080 x 2340**. W Unity sprawdzono również
zbliżoną proporcję **740 x 1636**.

### Panel Settings

Plik:

~~~text
Assets/DistrictEmpire/Presentation/UI/DistrictEmpirePanelSettings.asset
~~~

Wymagane ustawienia:

- Scale Mode: **Scale With Screen Size**.
- Reference Resolution: **540 x 960**.
- Screen Match Mode: **Match Width Or Height**.
- Match: **0**, czyli dopasowanie po szerokości.

Nie używaj **Constant Physical Size**. Na telefonie z wysokim DPI powiększy
to cały interfejs.

## Test logiki

W Unity uruchom:

~~~text
District Empire > Run Core Playtest
~~~

Poprawny wynik:

~~~text
District Empire core playtest passed
~~~

Test obejmuje czynsz, sklep, daily reward, naprawę, umowy, sprzedaż, event,
zakup, notariusza, ogłoszenie, wybór najemcy i reset profilu.

Dłuższy test ekonomii:

~~~text
District Empire > Simulate 30 Days
~~~

## Reset gry

W aplikacji:

~~~text
Shop > Reset local profile
~~~

Na podłączonym Androidzie można usunąć cały lokalny zapis:

~~~sh
"/Applications/Unity/Hub/Editor/6000.3.5f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb" \
  shell pm clear com.disordermail.districtempire
~~~

Uwaga: ta komenda bezpowrotnie usuwa dane gry z telefonu.

## Budowanie APK

W Unity wybierz:

~~~text
District Empire > Build Android APK
~~~

Aktualny plik wynikowy:

~~~text
unity/Builds/DistrictEmpire-0.6.apk
~~~

Przed kolejnym wydaniem zwiększ nazwę i kod wersji w:

~~~text
unity/Assets/Editor/BuildDistrictEmpireAndroid.cs
unity/ProjectSettings/ProjectSettings.asset
~~~

Build z terminala, po zamknięciu graficznego Unity:

~~~sh
"/Applications/Unity/Hub/Editor/6000.3.5f1/Unity.app/Contents/MacOS/Unity" \
  -batchmode \
  -nographics \
  -projectPath /Users/pwd/AI/DistrictEmpire/unity \
  -buildTarget Android \
  -executeMethod DistrictEmpire.EditorTools.BuildDistrictEmpireAndroid.BuildApk \
  -logFile /private/tmp/district-android-build.log \
  -quit
~~~

## Instalacja APK na telefonie

Włącz USB debugging, podłącz telefon i sprawdź połączenie:

~~~sh
"/Applications/Unity/Hub/Editor/6000.3.5f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb" \
  devices -l
~~~

Zainstaluj lub zaktualizuj APK:

~~~sh
"/Applications/Unity/Hub/Editor/6000.3.5f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb" \
  install -r /Users/pwd/AI/DistrictEmpire/unity/Builds/DistrictEmpire-0.6.apk
~~~

Identyfikator aplikacji:

~~~text
com.disordermail.districtempire
~~~

## Pełny restart Unity

Najpierw zamknij Unity normalnie. Jeśli edytor się zawiesił, znajdź procesy:

~~~sh
pgrep -fl "/Applications/Unity/Hub/Editor/6000.3.5f1/Unity.app/Contents/MacOS/Unity"
~~~

Zakończ tylko proces główny i workery zawierające:

~~~text
-projectPath /Users/pwd/AI/DistrictEmpire/unity
~~~

Potem ponownie wykonaj komendę z sekcji **Otwarcie Unity**. Nie usuwaj
folderów **Assets**, **Packages** ani **ProjectSettings**.

## Typowe problemy

### UI jest ogromne

Sprawdź Panel Settings. Najczęstszą przyczyną jest **Constant Physical Size**
zamiast **Scale With Screen Size**.

### Tekst nachodzi na elementy

Sprawdź **Free Aspect**. Długie nazwy powinny używać jednej linii i
wielokropka. Elastyczne kontenery potrzebują **min-width: 0** i
**flex-shrink**.

### Projekt jest już otwarty

Zamknij poprzedni edytor i jego **AssetImportWorker** dla tego samego
**projectPath**. Nie otwieraj projektu w dwóch instancjach Unity.

### Brakuje narzędzi Androida

W Unity Hub wybierz:

~~~text
Installs > Unity 6.3 LTS > Add modules
~~~

Zainstaluj Android Build Support, Android SDK & NDK Tools oraz OpenJDK.

### Build się nie udał

Sprawdź:

~~~text
/private/tmp/district-android-build.log
~~~

Wyszukaj **error CS**, **Exception**, **BUILD FAILED** lub
**Scripts have compiler errors**.

## Zapisanie zmian w Git

~~~sh
cd ~/AI/DistrictEmpire
git status
git diff --check
git add <zmienione-pliki>
git commit -m "Opis zmiany"
git push origin main
~~~

Nie dodawaj folderów **Library/**, **Temp/** ani lokalnych logów.

## Checklista

- [ ] Unity i moduły Android są zainstalowane.
- [ ] Otwarty jest folder **unity/**.
- [ ] Scena działa w Play Mode.
- [ ] UI działa w Free Aspect i proporcji telefonu.
- [ ] Core Playtest kończy się komunikatem **passed**.
- [ ] Numer wersji APK został zwiększony.
- [ ] APK zostało zbudowane i zainstalowane.
- [ ] Główny flow został sprawdzony na telefonie.
- [ ] Zmiany zostały zapisane w Git.
