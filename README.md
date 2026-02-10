# RefZero: .NET Reference Manager & Auditor

**RefZero**는 .NET 프로젝트의 참조(Reference)를 효율적으로 관리, 분석, 수집하기 위한 도구입니다. 복잡한 솔루션에서 불필요한 참조를 찾아내거나, 배포를 위해 모든 의존성 DLL을 한곳으로 모으는 작업을 자동화합니다.

## ✨ 주요 기능 (Key Features)

### 1. 📦 스마트 DLL 수집 (Smart DLL Collection)
빌드된 결과물뿐만 아니라 프로젝트가 참조하는 모든 DLL을 지정된 폴더로 수집합니다.
*   **프로젝트 참조 분리**: 솔루션 내의 프로젝트 참조(Project Reference)는 `refByPrj` 하위 폴더에 별도로 정리되어 섞이지 않습니다.
*   **바이너리 참조 통합**: NuGet 패키지나 외부 DLL 참조는 루트 폴더에 통합됩니다.

### 2. 🧹 미사용 참조 정리 (Unused Reference Cleaner)
프로젝트 소스 코드(.cs)를 분석하여 실제 코드에서 사용되지 않는 참조를 찾아냅니다.
*   **Roslyn 정적 분석**: C# 코드를 파싱하여 `using` 구문과 실제 사용된 타입을 추적합니다.
*   **GUI 통합**: 분석된 미사용 참조 목록을 시각적으로 확인하고, 체크박스를 통해 선택적으로 제거할 수 있습니다.

### 3. 🛡️ 강력한 호환성 (Cross-Runtime Compatibility)
**Out-of-Process** 아키텍처를 채택하여, 실행 환경의 제약을 극복했습니다.
*   **RefZero.GUI**: 친숙한 Windows Forms 기반 (.NET Framework 4.8) 인터페이스 제공.
*   **RefZero.CLI**: 최신 .NET 8.0/6.0 기반의 강력한 분석 엔진 탑재.
*   이 구조 덕분에 레거시 .NET Framework 프로젝트와 최신 .NET Core 프로젝트 모두를 안정적으로 분석할 수 있습니다.

---

## 🚀 시작하기 (Getting Started)

### 사전 요구 사항 (Prerequisites)
*   **Windows OS** (GUI 실행용)
*   **.NET Desktop Runtime 4.8** 이상
*   **.NET SDK 6.0** 또는 **8.0** 이상 (CLI 분석 엔진용)

### 설치 및 빌드 (Build)
이 리포지토리를 클론하고 솔루션을 빌드합니다.

```bash
git clone https://github.com/Start-to-End-AI-Coding/RefZero.git
cd RefZero
dotnet build RefZero.sln
```

---

## 📖 사용 방법 (Usage)

### 1. GUI 모드 (권장)
`RefZero.GUI.exe`를 실행하여 직관적으로 작업을 수행할 수 있습니다.

1.  **Project File**: `Browse` 버튼을 눌러 분석할 `.csproj` 파일을 선택합니다.
2.  **Analyz Only**: 프로젝트의 참조 현황을 로그 창에 출력합니다.
3.  **DLL Collector (탭)**:
    *   `Output Directory`: DLL을 모을 폴더를 선택합니다.
    *   `Collect`: 수집을 시작합니다. 완료되면 `Open Folder`로 결과를 확인하세요.
4.  **Unused Refs (탭)**:
    *   `Analyze Unused References`: 소스 코드를 분석하여 불필요한 참조를 찾습니다.
    *   `Select All` 체크박스로 전체 항목을 한 번에 선택/해제할 수 있습니다.
    *   목록에서 제거할 항목을 체크하고 `Remove Selected`를 클릭하면 `.csproj`에서 해당 참조가 삭제됩니다.

### 2. CLI 모드 (자동화용)
CI/CD 파이프라인이나 스크립트에서 자동화 작업을 수행할 때 유용합니다.

```bash
# 참조 수집
RefZero.CLI.exe collect -p "path/to/project.csproj" -o "path/to/output"

# 미사용 참조 분석 (JSON 출력)
RefZero.CLI.exe clean -p "path/to/project.csproj" --json --dry-run

# 참조 제거
RefZero.CLI.exe remove -p "path/to/project.csproj" -r "System.Data" -r "Newtonsoft.Json"
```

---

## 🏗️ 아키텍처 (Architecture)

RefZero는 **GUI**와 **CLI**가 분리된 구조를 가집니다.
*   **GUI**는 사용자 입력을 받아 **CLI**를 하위 프로세스로 실행합니다.
*   **CLI**는 MSBuild와 Roslyn을 사용하여 실제 프로젝트 파일을 로드하고 분석합니다.
*   이러한 접근 방식은 Visual Studio의 MSBuild 인스턴스와의 충돌을 방지하고, 다양한 .NET 버전 호환성을 보장합니다.

## 📝 라이선스 (License)
MIT License
