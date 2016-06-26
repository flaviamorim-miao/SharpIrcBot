﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Messenger.ORM
{
    [Table("private_messages", Schema = "messenger")]
    public class PrivateMessage : IMessage
    {
        [Key]
        [Required]
        [Column("message_id", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        [Column("timestamp", Order = 2)]
        public DateTimeOffset Timestamp { get; set; }

        [Required]
        [Column("sender_original", Order = 3)]
        [MaxLength(255)]
        public string SenderOriginal { get; set; }

        [Required]
        [Column("recipient_lowercase", Order = 4)]
        [MaxLength(255)]
        public string RecipientLowercase { get; set; }

        [Required]
        [Column("body", Order = 5)]
        public string Body { get; set; }

        public PrivateMessage()
        {
        }

        public PrivateMessage(IMessage other)
        {
            MessageUtils.TransferMessage(other, this);
        }
    }
}