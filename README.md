# 🚀 MES 실무 기반 백엔드 API (.NET 8)

## 📁 프로젝트 소개
MES/WMS 시스템에 사용되는 알림, 전자서명, 바코드 기능을 포함한 백엔드 API입니다.  
실제 현장에서 사용되는 트랜잭션 기반 SP 호출, FCM 알림, 전자서명 처리 로직을 포함합니다.

## 🛠 사용 기술
- ASP.NET Core 8
- Entity Framework Core
- Firebase FCM (OAuth2 + HTTP v1)
- SQLite (Flutter 로컬 연동용)
- JWT 인증 + 미들웨어 처리

## 🔑 주요 기능
- [x] JWT 로그인 및 인증 미들웨어
- [x] 전자서명 처리 + SP 연동
- [x] 알림 큐 저장 → 실패시 재전송 로직
- [x] FCM 연동 (access_token 발급 및 메시지 전송)
- [x] 로그 저장 및 트랜잭션 처리

## 🌐 연동 대상
- Flutter 기반 모바일 MES 앱 (자체 개발)
- Firebase 알림 시스템
- MSSQL + SQLite 병행 연동

## 📂 프로젝트 구조
