# BingleStarter

Unity를 이용해서 3 Match 퍼즐 게임을 개발하는 과정을 공유합니다.  
개발중인 개인 프로젝트의 실제 코드 일부를 발췌하여 개발과정을 공유하고자 합니다.

<img src="https://1.bp.blogspot.com/-_AgMscYYw6w/Xd5oHI0zW1I/AAAAAAAAAn8/0tuzT5w9Wr4MLU7TKPcqeqL9_t09wXgJQCLcBGAsYHQ/s1600/RespawnBlocksAfterClear.gif"></img>

### 구현 목표
* 3, 4, 5 기본 매칭 
* 3x3, 3x4, 3x5, 4x4, 4x5 매칭시 특수블럭 생성
* 다양한 (젤리, 스톤, 철창, 컬러) 블럭 지원
* 스테이지 파일 지원
* 스테이지별 미션, 점수 지원
* 부드러운 블럭 드롭 애니메이션 지원
* 매칭가능 블럭 알림 기능
* 이동 가능 블럭 없는 경우 블럭 섞기
* 핵주먹, 뒤섞기, 이동횟수 증가 등 아이템 지원
* 맵/스테이지 선택, 게임준비 화면 등 다양한 UI
* 파트클을 사용한 블럭 폭파 및 클리어 Effect
* 오브젝트 풀(Pool)을 사용한 게임오브젝트 재활용
* UML을 이용한 아키텍쳐 설계 (클래스 다이어그램, 시퀀스 다이어그램, 상태 다이어그램 제공)
* 모바일 해상도 Free 
* 안드로이드 앱 빌드 및 배포
* 그외 시간이 되면
  - GC 최소화
  - DrawCall 줄기기 등
  - Google Play Service 연동
  - 결제 연동
  - Unit Test

### 개발 환경
* __OS__ : MacOS Mojave 버전 10.14.5
* __개발도구__ : Unity 2019.2.10f1 Personal,  Visual Studio for Mac
* __기타__ : Photoshop CC 2018
* __형상관리__ : git version 2.23.0
* __소스 Repository__ : Github
* __UML 도구__ : StarUML

### 블로그 편집 환경
* __동영상 GIF 캡쳐__ : GIF Brewery
* __HTML & CSS 테스트__ : IntelliJ + Visual Studio Code for Mac, 크롬
* __이미지 캡쳐__ : MacOS 기본 캡쳐
* __기타__ : PowerPoint

### 소스 코드 다운로드  
모든 소스 코드와 문서는 아래에서 다운로드 할 수 있습니다.  
https://github.com/ninez-entertain/BingleStarter  
아래 git command로 복제(clone) 후 사용하세요.
```bash
git clone https://github.com/ninez-entertain/BingleStarter.git
```
또한, 각 진행 과정은 개별 branch로 제공되며 소스코드 다운로드 후에 각 스텝 별로 브랜치를 이용할 수 있습니다.  
예를 들어, 세번째 장의 소스를 보기 위해서 "step-3" 브랜치로 이동할 수 있습니다.
```bash
git checkout step-3
```
### UML 다이어그램 파일 보기  
 프로젝트 설계에 사용하는 UML 도구는 오픈소스 프로그램 StarUML을 사용합니다.  
https://sourceforge.net/projects/staruml/ 에서 프로그램을 다운받을 수 있습니다.

```bash
UML 파일 위치 : Assets/Doc/BingleStart.uml
```

---
문의사항 및 잘못된 부분, 추가 의견은 댓글 및 메일(ninez.entertain@gmail.com)으로 부탁드립니다.  
많은 지적과 관심 부탁드립니다^^
