# 1 "fuse-2.3.h"
# 1 "<built-in>"
# 1 "<command line>"
# 1 "fuse-2.3.h"
# 11 "fuse-2.3.h"
# 1 "/usr/include/asm/types.h" 1 3 4














# 12 "fuse-2.3.h" 2
# 31 "fuse-2.3.h"
struct fuse_attr {
        unsigned long long ino;
        unsigned long long size;
        unsigned long long blocks;
        unsigned long long atime;
        unsigned long long mtime;
        unsigned long long ctime;
        unsigned int atimensec;
        unsigned int mtimensec;
        unsigned int ctimensec;
        unsigned int mode;
        unsigned int nlink;
        unsigned int uid;
        unsigned int gid;
        unsigned int rdev;
};

struct fuse_kstatfs {
        unsigned long long blocks;
        unsigned long long bfree;
        unsigned long long bavail;
        unsigned long long files;
        unsigned long long ffree;
        unsigned int bsize;
        unsigned int namelen;
};
# 66 "fuse-2.3.h"
enum fuse_opcode {
        FUSE_LOOKUP = 1,
        FUSE_FORGET = 2,
        FUSE_GETATTR = 3,
        FUSE_SETATTR = 4,
        FUSE_READLINK = 5,
        FUSE_SYMLINK = 6,
        FUSE_MKNOD = 8,
        FUSE_MKDIR = 9,
        FUSE_UNLINK = 10,
        FUSE_RMDIR = 11,
        FUSE_RENAME = 12,
        FUSE_LINK = 13,
        FUSE_OPEN = 14,
        FUSE_READ = 15,
        FUSE_WRITE = 16,
        FUSE_STATFS = 17,
        FUSE_RELEASE = 18,
        FUSE_FSYNC = 20,
        FUSE_SETXATTR = 21,
        FUSE_GETXATTR = 22,
        FUSE_LISTXATTR = 23,
        FUSE_REMOVEXATTR = 24,
        FUSE_FLUSH = 25,
        FUSE_INIT = 26,
        FUSE_OPENDIR = 27,
        FUSE_READDIR = 28,
        FUSE_RELEASEDIR = 29,
        FUSE_FSYNCDIR = 30
};
# 104 "fuse-2.3.h"
struct fuse_entry_out {
        unsigned long long nodeid;
        unsigned long long generation;

        unsigned long long entry_valid;
        unsigned long long attr_valid;
        unsigned int entry_valid_nsec;
        unsigned int attr_valid_nsec;
        struct fuse_attr attr;
};

struct fuse_forget_in {
        unsigned long long version;
};

struct fuse_attr_out {
        unsigned long long attr_valid;
        unsigned int attr_valid_nsec;
        unsigned int dummy;
        struct fuse_attr attr;
};

struct fuse_mknod_in {
        unsigned int mode;
        unsigned int rdev;
};

struct fuse_mkdir_in {
        unsigned int mode;
        unsigned int padding;
};

struct fuse_rename_in {
        unsigned long long newdir;
};

struct fuse_link_in {
        unsigned long long oldnodeid;
};

struct fuse_setattr_in {
        unsigned int valid;
        unsigned int padding;
        struct fuse_attr attr;
};

struct fuse_open_in {
        unsigned int flags;
        unsigned int padding;
};

struct fuse_open_out {
        unsigned long long fh;
        unsigned int open_flags;
        unsigned int padding;
};

struct fuse_release_in {
        unsigned long long fh;
        unsigned int flags;
        unsigned int padding;
};

struct fuse_flush_in {
        unsigned long long fh;
        unsigned int flush_flags;
        unsigned int padding;
};

struct fuse_read_in {
        unsigned long long fh;
        unsigned long long offset;
        unsigned int size;
        unsigned int padding;
};

struct fuse_write_in {
        unsigned long long fh;
        unsigned long long offset;
        unsigned int size;
        unsigned int write_flags;
};

struct fuse_write_out {
        unsigned int size;
        unsigned int padding;
};

struct fuse_statfs_out {
        struct fuse_kstatfs st;
};

struct fuse_fsync_in {
        unsigned long long fh;
        unsigned int fsync_flags;
        unsigned int padding;
};

struct fuse_setxattr_in {
        unsigned int size;
        unsigned int flags;
};

struct fuse_getxattr_in {
        unsigned int size;
        unsigned int padding;
};

struct fuse_getxattr_out {
        unsigned int size;
        unsigned int padding;
};

struct fuse_init_in_out {
        unsigned int major;
        unsigned int minor;
};

struct fuse_in_header {
        unsigned int len;
        unsigned int opcode;
        unsigned long long unique;
        unsigned long long nodeid;
        unsigned int uid;
        unsigned int gid;
        unsigned int pid;
        unsigned int padding;
};

struct fuse_out_header {
        unsigned int len;
         int error;
        unsigned long long unique;
};

struct fuse_dirent {
        unsigned long long ino;
        unsigned long long off;
        unsigned int namelen;
        unsigned int type;
        char name[0];
};
