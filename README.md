# bankapi_ai
REST service for a banking use case. 
Project Requirements: 
Core Components: You must develop the following components: 
**Service Interface** 
- Signing up and Authentication
Use OAuth 2.0 for authentication. While generating token ensure to add some protection to avoid sharing same token from different IP addresses or browsers
- Account Holders 
Use AccountHolders table which should contain personal information and contact data in a single table. Key field is AccountHolderId. Separate AccountHolderId should be generated to be easily readable
API should provide operations for creation, mark for delete, editing of description
- Accounts 
This Accounts table should be linked to the AccountHolders withing foreign key AccountHolderId. Key field is AccountId and AccountNumber should be human readable. Also Account table should contains Balance field with decimal precision without data loss
API should provide operations for creation, mark for delete, editing of description
- Transactions
Containing table reflects transactions from Account to another Account using field SourceAccountId and TargetAccountId as well as Amount field with decimal precision without data loss
API should provide a way to create a transaction and retreive transaction by it's id
- Money Transfers 
This API should reflect operations within cards
- Cards 
This Cards table should be linked to the Accounts withing foreign key AccountId. Key field is CardId. CardNumber should be in a regular credit card format number, also all fields should be available.
API should contains operations for request a new card, block a card, temporarily block a card, check CVV code
- Statements 
This API reflects obtaining information from Transactions
 **Database** (using SQLite) 
- Database implementation should be in DDD using CQRS pattern
**Test Suite** 
- Comprehensive test coverage for critical functionality 
- Unit tests for business logic 
- Integration tests for API endpoints 
**Containerization** 
- Dockerfile for the application 
- docker-compose.yml for local development 
- Environment variable configuration 
- Multi-stage builds (bonus) 
**Logging & Monitoring** 
- Structured logging implementation 
- Log levels and formatting 
- Error tracking and reporting 
**Health Checks** 
- Health check endpoint 
- Database connectivity checks 
- Service readiness probes 
- Graceful shutdown handling
